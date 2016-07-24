using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BHOffice.Core.Linq
{

    public static class IQueryableExtension
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="start"></param>
        /// <param name="limit"></param>
        /// <param name="defaultStart">当start不符合格式时，使用此值</param>
        /// <param name="defaultLimit">当limit不符合格式时，使用此值</param>
        /// <returns></returns>
        public static IQueryable<T> Take<T>(this IQueryable<T> queryable, int? start, int? limit, int defaultStart = 0, int defaultLimit = 20)
        {
            start = start ?? new Nullable<int>(defaultStart);
            limit = limit ?? new Nullable<int>(defaultLimit);
            return Take(queryable, start.Value, limit.Value, defaultStart, defaultLimit);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="start"></param>
        /// <param name="limit"></param>
        /// <param name="defaultStart">当start不符合格式时，使用此值</param>
        /// <param name="defaultLimit">当limit不符合格式时，使用此值</param>
        /// <returns></returns>
        public static IQueryable<T> Take<T>(this IQueryable<T> queryable, int start, int limit, int defaultStart = 0, int defaultLimit = 20)
        {
            start = start < 0 ? defaultStart : start;
            limit = limit < 0 ? defaultLimit : limit;

            start = Math.Max(start, 0);
            limit = Math.Max(limit, 0);

            return queryable.Skip(start).Take(limit);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="defaultPageIndex">当start不符合格式时，使用此值</param>
        /// <param name="defaultPageSize">当limit不符合格式时，使用此值</param>
        /// <returns></returns>
        public static IQueryable<T> TakePage<T>(this IQueryable<T> queryable, int pageIndex, int pageSize, int defaultPageIndex = 1, int defaultPageSize = 20)
        {
            pageIndex = pageIndex < 0 ? defaultPageIndex : pageIndex;
            pageSize = pageSize < 0 ? defaultPageSize : pageSize;

            pageIndex = Math.Max(pageIndex, 1);
            pageSize = Math.Max(pageSize, 0);

            return queryable.Skip(pageSize * (pageIndex - 1)).Take(pageSize);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="defaultPageIndex">当start不符合格式时，使用此值</param>
        /// <param name="defaultPageSize">当limit不符合格式时，使用此值</param>
        /// <returns></returns>
        public static IQueryable<T> TakePage<T>(this IQueryable<T> queryable, int? pageIndex, int? pageSize, int defaultPageIndex = 1, int defaultPageSize = 20)
        {
            pageIndex = pageIndex ?? new Nullable<int>(defaultPageIndex);
            pageSize = pageSize ?? new Nullable<int>(defaultPageSize);
            return TakePage(queryable, pageIndex.Value, pageSize.Value, defaultPageIndex, defaultPageSize);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="count"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="defaultPageIndex"></param>
        /// <param name="defaultPageSize"></param>
        /// <param name="inScope">限制在最大页码内</param>
        /// <returns></returns>
        public static IQueryable<T> TakePage<T>(this IQueryable<T> queryable, out int count, int? pageIndex, int? pageSize, int defaultPageIndex = 1, int defaultPageSize = 20, bool inScope = true)
        {
            int pageCount;
            return TakePage(queryable, out count, out pageCount, pageIndex.Value, pageSize.Value, defaultPageIndex, defaultPageSize, inScope);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="count"></param>
        /// <param name="pageCount"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="defaultPageIndex"></param>
        /// <param name="defaultPageSize"></param>
        /// <param name="inScope">限制在最大页码内</param>
        /// <returns></returns>
        public static IQueryable<T> TakePage<T>(this IQueryable<T> queryable, out int count, out int pageCount, int? pageIndex, int? pageSize, int defaultPageIndex = 1, int defaultPageSize = 20, bool inScope = true)
        {
            pageIndex = pageIndex ?? new Nullable<int>(defaultPageIndex);
            pageSize = pageSize ?? new Nullable<int>(defaultPageSize);
            return TakePage(queryable, out count, out pageCount, pageIndex.Value, pageSize.Value, defaultPageIndex, defaultPageSize, inScope);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="count"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="defaultPageIndex"></param>
        /// <param name="defaultPageSize"></param>
        /// <param name="inScope">限制在最大页码内</param>
        /// <returns></returns>
        public static IQueryable<T> TakePage<T>(this IQueryable<T> queryable, out int count, int pageIndex, int pageSize, int defaultPageIndex = 1, int defaultPageSize = 20, bool inScope = true)
        {
            int pageCount;
            return TakePage(queryable, out count, out pageCount, pageIndex, pageSize, defaultPageIndex, defaultPageSize);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="count"></param>
        /// <param name="pageCount"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="defaultPageIndex"></param>
        /// <param name="defaultPageSize"></param>
        /// <param name="inScope">限制在最大页码内</param>
        /// <returns></returns>
        public static IQueryable<T> TakePage<T>(this IQueryable<T> queryable, out int count, out int pageCount, int pageIndex, int pageSize, int defaultPageIndex = 1, int defaultPageSize = 20, bool inScope = true)
        {
            count = queryable.Count();

            pageSize = pageSize < 0 ? defaultPageSize : pageSize;

            int result;
            pageCount = Math.DivRem(count, pageSize, out result);
            if (result > 0)
                pageCount++;

            if (inScope)
            {
                pageIndex = Math.Min(pageCount, pageIndex);
            }

            return TakePage(queryable, pageIndex, pageSize, defaultPageIndex, defaultPageSize);
        }

        /// <summary>
        /// 计算总数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static IQueryable<T> Count<T>(this IQueryable<T> queryable, out int count)
        {
            count = queryable.Count();
            return queryable;
        }

        /// <summary>
        /// 返回最大值，如果不包含值则返回默认值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <returns></returns>
        public static T? MaxOrDefault<T>(this IQueryable<T> queryable)
            where T : struct
        {
            return queryable.Max(i => (T?)i);
        }

        /// <summary>
        /// 返回最大值，如果不包含值则返回默认值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <returns></returns>
        public static T? MaxOrDefault<TScource, T>(this IQueryable<TScource> queryable, System.Linq.Expressions.Expression<Func<TScource, T>> selecter)
            where T : struct
        {
            return queryable.Select(selecter).MaxOrDefault();
        }

        /// <summary>
        /// 返回最小值，如果不包含值则返回默认值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <returns></returns>
        public static T? MinOrDefault<T>(this IQueryable<T> queryable)
            where T : struct
        {
            return queryable.Min(i => (T?)i);
        }

        /// <summary>
        /// 返回最大值，如果不包含值则返回默认值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <returns></returns>
        public static T? MinOrDefault<TScource, T>(this IQueryable<TScource> queryable, System.Linq.Expressions.Expression<Func<TScource, T>> selecter)
            where T : struct
        {
            return queryable.Select(selecter).MinOrDefault();
        }
    }
}
