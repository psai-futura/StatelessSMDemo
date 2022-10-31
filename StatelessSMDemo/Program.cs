using StatelessSMDemo;

var rfq = new Rfq();

var suppliersList = new List<Supplier>
{
    new(10, "Supplier1", "abc1@c.com"),
    new(11, "Supplier2", "abc2@c.com"),
    new(12,"Supplier3", "abc3@c.com"),
    new(13,"Supplier4", "abc4@c.com")
};

rfq.Suppliers = suppliersList;

var rfqStateMachine = new RfqStateMachine();
rfqStateMachine.SetRfq(rfq);

rfqStateMachine.Fire(RfqTriggers.Initialize);
rfqStateMachine.Fire(RfqTriggers.Start);
rfqStateMachine.Fire(RfqTriggers.Update);
rfqStateMachine.Fire(RfqTriggers.Cancel);
rfqStateMachine.Fire(RfqTriggers.Start);
rfqStateMachine.Fire(RfqTriggers.Close);
//Console.WriteLine($"Rfq State: {rfq.CurrentState}");

foreach (var state in rfq.StateHistory.GetStateHistory())
    Console.WriteLine($" RfqId {rfq.RfqId} : State - {state.CurrentStateDefinition} on {state.StateChangeDate:O}");

var rfqToSupplierStateMachine = new RfqToSupplierStateMachine();
rfqToSupplierStateMachine.SetRfqToSupplier(((IList<RfqToSupplier>)rfq.RfqToSuppliers)[0]);

rfqToSupplierStateMachine.Fire(RfqToSupplierTriggers.Open);

Console.WriteLine($"RfqToSupplier Id {((IList<RfqToSupplier>)rfq.RfqToSuppliers)[0].RfqToSupplierId} is " +
                  $"{((IList<RfqToSupplier>)rfq.RfqToSuppliers)[0].State}");













