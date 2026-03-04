using Radzen;
using Radzen.Blazor;
using StaticClass.Extensions;
using System.Collections;
using System.Linq.Expressions;
using System.Reflection;

namespace Website.Pages.Shared.Components
{
    public static class Extensions
    {
        private static readonly MethodInfo StringContainsMethod =
            typeof(string).GetMethod(nameof(string.Contains), new[] { typeof(string) })!;

        private static readonly MethodInfo StringToLowerMethod =
            typeof(string).GetMethod(nameof(string.ToLower), Type.EmptyTypes)!;

        private static readonly MethodInfo StringLatinizeMethod =
            typeof(StringExtensions).GetMethod(nameof(StringExtensions.Latinize),
                BindingFlags.Public | BindingFlags.Static, new[] { typeof(string) }) ?? null!;

        public static IQueryable? GetQuery(this IQueryable query, string propertyName, string searchValue, bool isCaseInsensitive)
        {
            var type = Type.GetType($"{query.ElementType.FullName}, {query.ElementType.Assembly.GetName().Name}")!;

            ParameterExpression parameter = Expression.Parameter(type, "x");

            var property = Expression.Property(parameter, propertyName);
            var valueExpression = Expression.Constant(isCaseInsensitive ? searchValue.ToLower().Latinize() : searchValue.Latinize());

            var RemoveDiacritics = Expression.Call(StringLatinizeMethod, property);

            Expression toLower = null!;
            if (isCaseInsensitive)
                toLower = Expression.Call(RemoveDiacritics, StringToLowerMethod);

            var call = Expression.Call(toLower ?? RemoveDiacritics, StringContainsMethod, valueExpression);

            var newQuery = typeof(Extensions)
            .GetMethod(nameof(CreateQuery), BindingFlags.NonPublic | BindingFlags.Static, new[] { typeof(IQueryable), typeof(MethodCallExpression), typeof(ParameterExpression) })?
            .MakeGenericMethod(type)
            .Invoke(null, new object[] { query, call, parameter });
            
            return (IQueryable?)newQuery;
        }

        private static IQueryable CreateQuery<T>(IQueryable query, MethodCallExpression call, ParameterExpression parameter)// where T : class 
        {
            dynamic dynamicQuery = query;
            return Queryable.Where(dynamicQuery, Expression.Lambda<Func<T, bool>>(call, parameter));
        }
    }

    public class CustomRadzenDropDown<TValue> : RadzenDropDown<TValue>
    {
        protected override IEnumerable? View
        {
            get
            {


                if (_view == null && Query != null)
                {
                    var searchText = base.GetType().GetField("searchText", BindingFlags.Instance
                        | BindingFlags.Public
                        | BindingFlags.NonPublic)?.GetValue(this)?.ToString();

                    if (!LoadData.HasDelegate && !string.IsNullOrEmpty(searchText))
                    {
                        bool isCaseInsensitive = FilterCaseSensitivity == FilterCaseSensitivity.CaseInsensitive;

                        _view = Query.GetQuery(TextProperty, searchText, isCaseInsensitive);
                    }
                    else
                    {
                        _view = base.View.AsQueryable();
                    }
                }

                return _view;
            }
        }

    }
}
