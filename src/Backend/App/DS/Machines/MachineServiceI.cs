using Backend.App.DS.Machines.Ent;

namespace Backend.App.DS.Machines
{
    public interface MachineServiceI
    {
        Task<List<Machine>> GetMachines(SessionEnt session);
    }
}
