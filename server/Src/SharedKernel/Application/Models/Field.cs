using SharedKernel.Libraries;

namespace SharedKernel.Application
{
    public class Field
    {
        private string _fieldName { get; set; }
        public string FieldName
        {
            get
            {
                if (!string.IsNullOrEmpty(_fieldName))
                {
                    if (_fieldName[0] >= 97 && _fieldName[0] <= 122)
                    {
                        _fieldName = char.ToUpper(_fieldName[0]) + _fieldName.Substring(1);
                    }
                }
                return _fieldName;
            }
            set
            {
                _fieldName = value;
            }
        }

        private string _value { get; set; }
        public string Value
        {
            get
            {
                if (_value == null)
                {
                    return string.Empty;
                }
                return _value;
            }
            set
            {
                _value = value;
            }
        }

        public WhereType Condition { get; set; } = WhereType.E;

        public string GetOperatorWithValue(out string paramName, bool hasUnicode = false)
        {
            paramName = $"{FieldName}{Utility.RandomString(6, false)}";
            //var suffix = hasUnicode ? "COLLATE utf8mb3_tolower_ci" : "";
            var suffix = string.Empty;

            switch (Condition)
            {
                case WhereType.E:
                    return $"= @{paramName}";
                case WhereType.NE:
                    return $"<> @{paramName}";
                case WhereType.GT:
                    return $"> @{paramName}";
                case WhereType.GE:
                    return $">= @{paramName}";
                case WhereType.LT:
                    return $"< @{paramName}";
                case WhereType.LE:
                    return $"<= @{paramName}";
                case WhereType.C:
                    return $"LIKE CONCAT('%', @{paramName} {suffix}, '%')";
                case WhereType.NC:
                    return $"NOT LIKE CONCAT('%', @{paramName} {suffix}, '%')";
                case WhereType.SW:
                    return $"LIKE CONCAT(@{paramName} {suffix}, '%')";
                case WhereType.NSW:
                    return $"NOT LIKE CONCAT(@{paramName} {suffix}, '%')";
                case WhereType.EW:
                    return $"LIKE CONCAT('%', @{paramName} {suffix})";
                case WhereType.NEW:
                    return $"NOT LIKE CONCAT('%', @{paramName} {suffix})";
                default:
                    return string.Empty;
            }
        }
    }
}
