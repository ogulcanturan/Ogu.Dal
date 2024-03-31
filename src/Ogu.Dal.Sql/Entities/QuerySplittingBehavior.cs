﻿#if NETSTANDARD2_0
namespace Ogu.Dal.Sql.Entities
{
    public enum QuerySplittingBehavior
    {
        //
        // Summary:
        //     The related collections will be loaded in same database query as parent query.
        //     This behavior generally guarantees result consistency in the face of concurrent
        //     updates (but details may vary based on the database and transaction isolation
        //     level in use). However, this can cause performance issues when the query loads
        //     multiple related collections.
        SingleQuery,
        //
        // Summary:
        //     The related collections will be loaded in separate database queries from the
        //     parent query.
        //     This behavior can significantly improve performance when the query loads multiple
        //     collections. However, since separate queries are used, this can result in inconsistent
        //     results when concurrent updates occur. Serializable or snapshot transactions
        //     can be used to mitigate this and achieve consistency with split queries, but
        //     that may bring other performance costs and behavioral difference.
        SplitQuery
    }
}
#endif