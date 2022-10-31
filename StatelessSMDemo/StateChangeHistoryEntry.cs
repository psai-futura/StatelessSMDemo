namespace StatelessSMDemo;

public class StateHistory
{
    private IEnumerable<StateChangeHistoryEntry> StateHistoryEntries { get; } = new List<StateChangeHistoryEntry>();

    public void Add(StateDefinition stateDefinition)
    {
        ArgumentNullException.ThrowIfNull(stateDefinition);

        var stateHistoryEntry = new StateChangeHistoryEntry(stateDefinition);
        ((IList<StateChangeHistoryEntry>)StateHistoryEntries).Add(stateHistoryEntry);
    }

    public IEnumerable<StateChangeHistoryEntry> GetStateHistory()
    {
        return StateHistoryEntries.OrderBy(t => t.StateChangeDate);
    }


    public StateDefinition GetCurrentState() => GetStateHistory().LastOrDefault()?.CurrentStateDefinition ?? new StateDefinition(RfqStates.NotStarted);

}

public class StateChangeHistoryEntry 
{

    public StateDefinition CurrentStateDefinition { get; }
        
    public DateTimeOffset StateChangeDate { get; }

    public StateChangeHistoryEntry(StateDefinition currentStateDefinition)
    {
        CurrentStateDefinition = currentStateDefinition;
        StateChangeDate = DateTimeOffset.UtcNow;
    }
}