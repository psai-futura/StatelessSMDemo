namespace StatelessSMDemo;

public class Rfq
{
    public Guid RfqId { get; } = Guid.NewGuid();
    
    public IEnumerable<Supplier> Suppliers { get; set; } = new List<Supplier>();
    
    public IEnumerable<RfqToSupplier> RfqToSuppliers { get; } = new List<RfqToSupplier>();


    public StateDefinition CurrentState
    {
        get => StateHistory.GetCurrentState();
        set => StateHistory.Add(value);
    }
    public StateHistory StateHistory { get; } = new();

    
    public void CreateRfqToSupplier()
    {
        var rfqToSupplier = new RfqToSupplier();
        ((IList<RfqToSupplier>)RfqToSuppliers).Add(rfqToSupplier);
    }


    // public void Cancel()
    // {
    //     if (State == RfqStateMachine.RfqStates.Closed)
    //         throw new InvalidOperationException("Cannot cancel a Closed RFQ.");
    //
    //     RfqToSuppliers.ForEach(rfqToSupplier => rfqToSupplier.State = RfqToSupplierStates.Cancelled);
    // }

    // public void SendEmailToSuppliers()
    // {
    //     foreach (var supplier in RfqToSuppliers)
    //         Console.WriteLine($"Email sent to Supplier {supplier.SupplierId}");
    // }
    
    // public void AddSupplier(int supplierId)
    // {
    //     if (State == RfqStateMachine.RfqStates.Closed)
    //         throw new InvalidOperationException("Supplier cannot be added to a Closed RFQ.");
    //
    //     Supplier supplier = new Supplier(supplierId);
    //     
    // }
}