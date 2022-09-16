namespace MouseControl
{
    public class TowerSelectable : BaseSelectable
    {
        public override void Select()
        {
            _selected = true;
        }

        public override void Deselect()
        {
            _selected = false;
        }
    }
}