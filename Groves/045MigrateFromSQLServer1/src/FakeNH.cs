using System;
using System.Linq.Expressions;

namespace ConsoleApplication
{
    #region dummy classes
    // there's no NH for .net core yet afaik, so I'm using these as placeholders to simulate
    public class ClassMap<T>
    {
        protected void Id(Expression<Func<T, object>> memberExpression) { throw new NotImplementedException(); }
        protected void Map(Expression<Func<T, object>> memberExpression) { throw new NotImplementedException(); }
        protected void HasMany(Expression<Func<T, object>> memberExpression) { throw new NotImplementedException(); }
        protected void UseTable(string tableName) { throw new NotImplementedException(); }
    }
    #endregion
}
