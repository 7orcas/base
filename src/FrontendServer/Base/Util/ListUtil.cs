using static MudBlazor.CategoryTypes;

namespace FrontendServer.Base.Util
{
    public class ListUtil<T> where T : Common.DTO._BaseDto
    {
        public int MaxSelections { get; set; } = 2;
        public bool ShowList { get; set; } = true;
        private List<ListModel<T>> Selections = new List<ListModel<T>>();

        public void Select(ListModel<T> model)
        {
            if (model.IsSelected)
            {
                model.IsSelected = false;
                var index = Selections.FindIndex(s => s.Id == model.Id);
                Selections.RemoveAt(index);
            }
            else
            {
                if (Selections.Count >= MaxSelections)
                {
                    Selections[0].IsSelected = false;
                    Selections.RemoveAt(0);
                }

                model.IsSelected = true;
                Selections.Add(model);
            }
        }
    }
}
