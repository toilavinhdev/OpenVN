using SharedKernel.Application;
using SharedKernel.Runtime.Exceptions;

namespace SharedKernel.Libraries
{
    public static class Flattener
    {
        private static int level = 1;

        public static List<IBaseTree> Flatten(IBaseTree tree, int? maxDeepLevel = default)
        {
            var result = new List<IBaseTree>() { tree };
            var code = Utility.RandomString(12);

            tree.Code = code;
            if (tree.Subs != null && tree.Subs.Any())
            {
                if (maxDeepLevel != null)
                {
                    level++;
                    if (level > maxDeepLevel.Value)
                    {
                        throw new BadRequestException($"We don't support deep level greater than {maxDeepLevel}");
                    }
                }

                foreach (var sub in tree.Subs)
                {
                    sub.RefCode = tree.Code;
                    result.AddRange(Flatten(sub, maxDeepLevel));
                }
            }
            return result;
        }
    }
}
