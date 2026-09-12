using System.ComponentModel;

namespace SJInovacao.Acesso.Modules.UserAccess.Domain.Common.Enumeradors
{
    public static class Enumerador
    {
        public static string? Descricao<T>(this T e) where T : IConvertible
        {
            if (e is Enum)
            {
                var type = e.GetType();
                var name = type.GetEnumName(e);
                var membro = type.GetMember(name);

                var descriptionAttribute = membro[0]
                    .GetCustomAttributes(typeof(DescriptionAttribute), false)
                    .FirstOrDefault() as DescriptionAttribute;

                return descriptionAttribute?.Description ?? name;
            }

            return null;
        }

        public static Dictionary<int, string> ParaLista<T>() where T : Enum
        {
            var type = typeof(T);
            var valores = Enum.GetValues(type).Cast<T>();

            return valores.ToDictionary(
                valor => Convert.ToInt32(valor),
                valor => valor.Descricao() ?? valor.ToString()
            );
        }
    }
}
