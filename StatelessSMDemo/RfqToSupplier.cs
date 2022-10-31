using System.Data.Common;

namespace StatelessSMDemo;

public class RfqToSupplier
{
    public Guid RfqToSupplierId { get; set; } = Guid.NewGuid();

    public RfqToSupplierStates State  { get; internal set; }

    private IEnumerable<StateChangeHistoryEntry> RfqStateChangeHistory { get; } = new List<StateChangeHistoryEntry>();
    
    public RfqToSupplier(RfqToSupplierStates state = RfqToSupplierStates.NotSeen)
    {
        State = state;
    }

    public void AddStateChangeRecord(StateChangeHistoryEntry stateChange)
    {
        ArgumentNullException.ThrowIfNull(stateChange);

        ((IList<StateChangeHistoryEntry>)RfqStateChangeHistory).Add(stateChange);
    }
}