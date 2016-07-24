using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BHOffice.Core
{
    /// <summary>
    /// 键值对集合的扩展方法
    /// </summary>
    public static class DictionaryExtension
    {
        /// <summary>
        /// 获取对应值，如果不存在返回默认值
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="self"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static V GetOrDefault<K, V>(this IDictionary<K, V> self, K key)
        {
            V v;
            if (!self.TryGetValue(key, out v))
            {
                v = default(V);
            }
            return v;
        }
    }
}
