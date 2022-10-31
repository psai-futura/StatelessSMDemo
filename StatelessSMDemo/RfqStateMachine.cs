using Stateless;

namespace StatelessSMDemo;

[Flags]
public enum Permissions
{
    View = 1 << 0,
    Update  = 1 << 1
}

public static class StateMachineExtension 
{
    public static StateMachine<TStates, TTriggers>.StateConfiguration AllowedPermissions<TStates, TTriggers>(this StateMachine<TStates, TTriggers>.StateConfiguration stateConfiguration, 
                                                                                                             Permissions permissions)
    {
        if (stateConfiguration.State is not StateDefinition stateDefinition) return stateConfiguration;
        
        stateDefinition.Permissions = permissions;

        return stateConfiguration;
    }

    public static StateMachine<StateDefinition, TTriggers>.StateConfiguration Configure<TTriggers>(this StateMachine<StateDefinition, TTriggers> stateMachine, Enum state)
    {
        return stateMachine.Configure(new StateDefinition(state));
    }
    
    public static StateMachine<StateDefinition, TTriggers>.StateConfiguration Permit<TTriggers>(this StateMachine<StateDefinition, TTriggers>.StateConfiguration stateConfiguration, 
                                                                                                TTriggers trigger, Enum destinationState)
    {
        return stateConfiguration.Permit(trigger, new StateDefinition(destinationState));
    }
}

public class StateDefinition
{
    private Enum _state; 
    public Enum State
    {
        get => _state;
        set => _state = value;
    }

    public Permissions Permissions { get; set; }
    
    public StateDefinition(Enum stateEnum)
    {
        _state = stateEnum;
    }

    public override bool Equals(object? obj)
    {
        if (obj is not StateDefinition stateDefinition) return false;

        var isSameState = stateDefinition.State.Equals(State);
        return isSameState;
    }

    public override int GetHashCode()
    {
        return State.GetHashCode();
    }

    public override string ToString()
    {
        return State.ToString();
    }
}

public enum RfqStates
{
    NotStarted,
    Draft,
    Started,
    Updated,
    Cancelled,
    Closed
}
    
public enum RfqTriggers
{
    Initialize,
    Start,
    Update,
    Cancel,
    Close
}

public class RfqStateMachine
{
    private StateMachine<StateDefinition, RfqTriggers>? _rfqStateMachine;

    private Rfq Rfq { get; set; }
    
    public void SetRfq(Rfq rfq)
    {
        Rfq = rfq;
        _rfqStateMachine = CreateStateMachine(Rfq);
    }

    private StateMachine<StateDefinition, RfqTriggers> CreateStateMachine(Rfq rfq)
    {
        var stateMachine = new StateMachine<StateDefinition, RfqTriggers>(()=> rfq.CurrentState, s =>
        {
            rfq.CurrentState = s;
            rfq.CurrentState.Permissions = s.Permissions;
        });
       
        stateMachine.Configure(RfqStates.NotStarted)
                    .Permit(RfqTriggers.Initialize, RfqStates.Draft);
        
        stateMachine.Configure(RfqStates.Draft)
                    .Permit(RfqTriggers.Start, RfqStates.Started)
                    .OnEntry(r => Console.WriteLine($"New RFQ with Id : {rfq.RfqId} created and is in {r.Destination} State"));

        stateMachine.Configure(RfqStates.Started)
                    .Permit(RfqTriggers.Update, RfqStates.Updated)
                    .Permit(RfqTriggers.Cancel, RfqStates.Cancelled)
                    .Permit(RfqTriggers.Close, RfqStates.Closed)
                    .AllowedPermissions(Permissions.View | Permissions.Update)
                    .OnEntry(() => GenerateRfqToSuppliers(rfq.Suppliers))
                    .OnExit(() => SendEmailToSuppliers(rfq.Suppliers));

        stateMachine.Configure(RfqStates.Updated)
                    .PermitReentry(RfqTriggers.Update)
                    .Permit(RfqTriggers.Cancel, RfqStates.Cancelled)
                    .Permit(RfqTriggers.Close, RfqStates.Closed)
                    .AllowedPermissions(Permissions.View | Permissions.Update);
            
        stateMachine.Configure(RfqStates.Cancelled)
                    .AllowedPermissions(Permissions.View)
                    .Permit(RfqTriggers.Close, RfqStates.Closed);
        
        stateMachine.Configure(RfqStates.Closed)
                    .AllowedPermissions(Permissions.View);

        stateMachine.OnUnhandledTrigger(UnhandledTriggerAction);
        stateMachine.OnTransitionCompleted(OnTransition);
        
        return stateMachine;
    }

    private static void UnhandledTriggerAction(StateDefinition stateDefinition, RfqTriggers trigger)
    {
        Console.WriteLine($"Unhandled: '{stateDefinition}' state, '{trigger}' trigger!");
    }

    public void Fire(RfqTriggers trigger)
    {
        ArgumentNullException.ThrowIfNull(_rfqStateMachine);
        
        if (_rfqStateMachine.CanFire(trigger))
            _rfqStateMachine.Fire(trigger);
        else
            Console.WriteLine($"Trigger {trigger} is not permitted when Rfq is in {Rfq.CurrentState}");
    }

    private void OnTransition(StateMachine<StateDefinition, RfqTriggers>.Transition transition)
    {
        Rfq.CurrentState = transition.Destination;
    }

    private void GenerateRfqToSuppliers(IEnumerable<Supplier> supplierList)
    {
        foreach (var supplier in supplierList)
            Rfq.CreateRfqToSupplier();
        
        foreach (var rfqToSupplier in Rfq.RfqToSuppliers)
            Console.WriteLine($"RfqToSupplier Id: {rfqToSupplier.RfqToSupplierId} is in {rfqToSupplier.State}");
    }

    private static void SendEmailToSuppliers(IEnumerable<Supplier> supplierList)
    {
        foreach (var supplier in supplierList)
            Console.WriteLine($"Email sent to {supplier.Email}");
    }
}