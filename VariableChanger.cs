using System.Collections.Generic;

using MSCLoader;

namespace Adrenaline
{
    internal class VariableChanger
    {
        private string field;
        private string ID;
        private List<SettingsSlider> _sliders = null;

        internal VariableChanger(string name, ref List<SettingsSlider> t)
        {
            this.field = name;
            this.ID = name.GetHashCode().ToString();
            this._sliders = t;
        }

        internal void ValueChanged()
        {
            var slider = _sliders.Find(v => v.Instance.ID == ID);
            AdrenalineLogic.config[field] = slider.GetValue();
            Utils.PrintDebug("Set value for " + field + " == " + slider.GetValue());
        }
    }
}
