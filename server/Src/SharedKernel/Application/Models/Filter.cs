using SharedKernel.Libraries;

namespace SharedKernel.Application
{
    public class Filter
    {
        public List<Field> Fields { get; set; }

        private string _formula { get; set; }

        public string Formula
        {
            get
            {
                if (string.IsNullOrEmpty(_formula))
                {
                    BuildFormula();
                }
                return StringHelper.RemoveExtraWhitespace(_formula);
            }
            set
            {
                _formula = value;
            }
        }

        private void BuildFormula()
        {
            if (Fields != null && Fields.Any())
            {
                var indexs = new List<string>();
                for (int i = 0; i < Fields.Count; i++)
                {
                    indexs.Add("{" + i + "}");
                }
                _formula = string.Join(" AND ", indexs.ToArray());
            }
        }
    }
}
