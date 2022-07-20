using System.Globalization;
using System.Text.RegularExpressions;

namespace WebOverlay.Html
{
    public delegate bool AttributeValidationRuleCallback(object value);
    public class AttributeValidationRule
    {
        protected readonly bool m_IsEditable = false;
        private readonly AttributeValidationRuleCallback m_Callback;

        public AttributeValidationRule(AttributeValidationRuleCallback callback, bool isEditable = false)
        {
            m_IsEditable = isEditable;
            m_Callback = callback;
        }

        public bool IsEditable() { return m_IsEditable; }
        public virtual bool IsValid(object value) { return m_Callback(value); }
    }

    public class IsTypeRule<T>: AttributeValidationRule
    {
        public IsTypeRule(bool isEditable = false): base(value => value is T, isEditable) {}
    }

    public class IsNonEmptyStringRule : AttributeValidationRule
    {
        public IsNonEmptyStringRule(bool isEditable = false) : base(value => value is string str && !string.IsNullOrWhiteSpace(str), isEditable) { }
    }

    public class IsLangRule: AttributeValidationRule
    {
        public IsLangRule(bool isEditable = false) : base(value => {
            if (value is string str && !string.IsNullOrWhiteSpace(str))
            {
                try
                {
                    RegionInfo info = new(str);
                    return true;
                }
                catch { }
            }
            return false;
        }, isEditable) { }
    }

    public class IsMatchingRegexRule : AttributeValidationRule
    {
        private readonly Regex m_Regex;

        public IsMatchingRegexRule(string regex, bool isEditable = false) : base(value => false, isEditable)
        {
            m_Regex = new(regex);
        }

        public override bool IsValid(object value)
        {
            return value is string str && m_Regex.IsMatch(str);
        }
    }

    public class IsInListRule<T> : AttributeValidationRule
    {
        private readonly List<T> m_List;

        public IsInListRule(List<T> list, bool isEditable = false) : base(value => false, isEditable)
        {
            m_List = list;
        }

        public override bool IsValid(object value)
        {
            return value is T obj && m_List.Contains(obj);
        }
    }
}
