namespace Backend.App.DS.Machines.Ent
{
    public class Machine : BaseEntity<Machine>
    {
        public int StationPairs { get; set; }

        public override void Decode() { }
        public override void Encode() { }

    }
}
