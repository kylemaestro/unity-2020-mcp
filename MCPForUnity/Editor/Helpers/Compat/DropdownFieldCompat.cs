using System.Linq;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace MCPForUnity.Editor.Helpers.Compat
{
    /// <summary>
    /// UXML-instantiable dropdown that works on Unity 2020.3, where
    /// UnityEngine.UIElements.DropdownField does not exist yet.
    /// </summary>
    public class DropdownField : PopupField<string>
    {
        public new class UxmlFactory : UxmlFactory<DropdownField, UxmlTraits> { }

        public new class UxmlTraits : BaseField<string>.UxmlTraits
        {
            private readonly UxmlStringAttributeDescription m_Choices = new UxmlStringAttributeDescription { name = "choices" };
            private readonly UxmlIntAttributeDescription m_Index = new UxmlIntAttributeDescription { name = "index", defaultValue = -1 };

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                var df = (DropdownField)ve;
                string choicesStr = m_Choices.GetValueFromBag(bag, cc);
                if (!string.IsNullOrEmpty(choicesStr))
                {
                    df.choices = choicesStr.Split(',').Select(s => s.Trim()).ToList();
                }
                int idx = m_Index.GetValueFromBag(bag, cc);
                if (idx >= 0 && df.choices != null && idx < df.choices.Count)
                {
                    df.index = idx;
                }
            }
        }

        public DropdownField()
        {
        }
    }
}
