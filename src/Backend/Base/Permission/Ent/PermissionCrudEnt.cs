using GC = Backend.GlobalConstants;

namespace Backend.Base.Permission.Ent
{
    /// <summary>
    /// Resolved login / permission number / crud for the logged in org
    /// </summary>
    /// <author>John Stewart</author>
    /// <created>April 13, 2025</created>
    /// <license>**Licence**</license>
    public class PermissionCrudEnt
    {
        public PermissionCrudEnt() { }
        public int Nr {  get; set; }
        public string Crud { get; set; }

        public void AddCrud(string crud)
        {
            if (string.IsNullOrEmpty(crud)) return;
            if (string.IsNullOrEmpty(Crud)) Crud = crud;
           
            for (int i = 0; i < crud.Length; i++)
                if (!Crud.Contains(crud[i])) Crud += crud[i];
           
            var crudx = "";
            if (Crud.Contains(GC.CrudCreate)) crudx += GC.CrudCreate;
            if (Crud.Contains(GC.CrudRead)) crudx += GC.CrudRead;
            if (Crud.Contains(GC.CrudUpdate)) crudx += GC.CrudUpdate;
            if (Crud.Contains(GC.CrudDelete)) crudx += GC.CrudDelete;
            Crud = crudx;
        }

    }
}
