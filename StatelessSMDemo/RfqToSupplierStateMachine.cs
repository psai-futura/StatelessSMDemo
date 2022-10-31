using Stateless;

namespace StatelessSMDemo;

public enum RfqToSupplierStates
{
    NotSeen,
    Seen,
    Accepted,
    Rejected,
    Cancelled,
    Closed
}
    
public enum RfqToSupplierTriggers
{
    Open,
    Accept,
    Reject,
    Cancel,
    Close
}

public class RfqToSupplierStateMachine
{
    private StateMachine<RfqToSupplierStates, RfqToSupplierTriggers>? _stateMachine;
    
    private RfqToSupplier _rfqToSupplier { get; set; }
    

    public void SetRfqToSupplier(RfqToSupplier rfqToSupplier)
    {
        _rfqToSupplier = rfqToSupplier;
        _stateMachine = CreateStateMachine(_rfqToSupplier);
    }
    
    private StateMachine<RfqToSupplierStates, RfqToSupplierTriggers> CreateStateMachine(RfqToSupplier rfqToSupplier)
    {
        var stateMachine = new StateMachine<RfqToSupplierStates, RfqToSupplierTriggers>(() => rfqToSupplier.State, s => rfqToSupplier.State = s );

        stateMachine.Configure(RfqToSupplierStates.NotSeen)
            .Permit(RfqToSupplierTriggers.Open, RfqToSupplierStates.Seen);

        stateMachine.Configure(RfqToSupplierStates.Seen)
            .Permit(RfqToSupplierTriggers.Accept, RfqToSupplierStates.Accepted)
            .Permit(RfqToSupplierTriggers.Reject, RfqToSupplierStates.Rejected);

        stateMachine.Configure(RfqToSupplierStates.Accepted)
            .Permit(RfqToSupplierTriggers.Close, RfqToSupplierStates.Closed);
        
        stateMachine.Configure(RfqToSupplierStates.Rejected);
        
        stateMachine.Configure(RfqToSupplierStates.Cancelled);
        
        stateMachine.Configure(RfqToSupplierStates.Closed);
        
        return stateMachine;
    }
    
    public void Fire(RfqToSupplierTriggers trigger)
    {
        ArgumentNullException.ThrowIfNull(_stateMachine);
        lock (_stateMachine)
        {
            // The state machine will throw an exception if you call Fire when the 
            // trigger is not allowed.
            if (_stateMachine.CanFire(trigger))
                _stateMachine.Fire(trigger);
            else
                Console.WriteLine($"Trigger {trigger} is not permitted when Rfq is in {_rfqToSupplier.State}");
            
        } 
    }
    
    private void OnTransition(StateMachine<StateDefinition, RfqToSupplierTriggers>.Transition transition)
    {
        _rfqToSupplier.AddStateChangeRecord( new StateChangeHistoryEntry(transition.Destination));
    }

}