using Backend.App.Machines.Ent;

namespace Backend.App.Machines
{
    public interface MachineServiceI
    {
        Task<List<Machine>> GetMachines(SessionEnt session);
    }
}
