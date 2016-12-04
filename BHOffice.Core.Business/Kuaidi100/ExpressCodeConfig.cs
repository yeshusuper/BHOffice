using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace BHOffice.Core.Business.Kuaidi100
{
    public class ExpressCodeConfig
    {
        private static ConcurrentDictionary<string, string> Kuaidi100;
        static ExpressCodeConfig()
        {
            Kuaidi100 = new ConcurrentDictionary<string, string>();
            Kuaidi100.GetOrAdd("中邮", "zhongyouwuliu");
            Kuaidi100.GetOrAdd("中通", "zhongtong");
            Kuaidi100.GetOrAdd("中铁", "zhongtiewuliu");
            Kuaidi100.GetOrAdd("宅急送", "zhaijisong");
            Kuaidi100.GetOrAdd("运通", "yuntongkuaidi");
            Kuaidi100.GetOrAdd("源安达", "yuananda");
            Kuaidi100.GetOrAdd("韵达", "yunda");
            Kuaidi100.GetOrAdd("越丰", "yuefengwuliu");
            Kuaidi100.GetOrAdd("元智捷诚", "yuanzhijiecheng");
            Kuaidi100.GetOrAdd("源伟丰", "yuanweifeng");
            Kuaidi100.GetOrAdd("圆通", "yuantong");
            Kuaidi100.GetOrAdd("远成", "yuanchengwuliu");
            Kuaidi100.GetOrAdd("优速", "youshuwuliu");
            Kuaidi100.GetOrAdd("一邦", "yibangwuliu");
            Kuaidi100.GetOrAdd("亚风", "yafengsudi");
            Kuaidi100.GetOrAdd("鑫飞鸿", "xinhongyukuaidi");
            Kuaidi100.GetOrAdd("星晨急便", "xingchengjibian");
            Kuaidi100.GetOrAdd("信丰", "xinfengwuliu");
            Kuaidi100.GetOrAdd("新邦", "xinbangwuliu");
            Kuaidi100.GetOrAdd("万象", "wanxiangwuliu");
            Kuaidi100.GetOrAdd("伍圆", "wuyuansudi");
            Kuaidi100.GetOrAdd("文捷", "wenjiesudi");
            Kuaidi100.GetOrAdd("万家", "wanjiawuliu");
            Kuaidi100.GetOrAdd("ups", "ups");
            Kuaidi100.GetOrAdd("tnt", "tnt");
            Kuaidi100.GetOrAdd("天天", "tiantian");
            Kuaidi100.GetOrAdd("天地华宇", "tiandihuayu");
            Kuaidi100.GetOrAdd("盛丰", "shengfengwuliu");
            Kuaidi100.GetOrAdd("速尔", "suer");
            Kuaidi100.GetOrAdd("盛辉", "shenghuiwuliu");
            Kuaidi100.GetOrAdd("全一", "quanyikuaidi");
            Kuaidi100.GetOrAdd("全日通", "quanritongkuaidi");
            Kuaidi100.GetOrAdd("全际通", "quanjitong");
            Kuaidi100.GetOrAdd("全晨", "quanchenkuaidi");
            Kuaidi100.GetOrAdd("配思", "peisihuoyunkuaidi");
            Kuaidi100.GetOrAdd("民航", "minghangkuaidi");
            Kuaidi100.GetOrAdd("龙邦", "longbanwuliu");
            Kuaidi100.GetOrAdd("联昊通", "lianhaowuliu");
            Kuaidi100.GetOrAdd("快捷", "kuaijiesudi");
            Kuaidi100.GetOrAdd("加运美", "jiayunmeiwuliu");
            Kuaidi100.GetOrAdd("佳吉", "jiajiwuliu");
            Kuaidi100.GetOrAdd("急先达", "jixianda");
            Kuaidi100.GetOrAdd("京广", "jinguangsudikuaijian");
            Kuaidi100.GetOrAdd("佳怡", "jiayiwuliu");
            Kuaidi100.GetOrAdd("华夏龙", "huaxialongwuliu");
            Kuaidi100.GetOrAdd("恒路", "hengluwuliu");
            Kuaidi100.GetOrAdd("汇通", "huitongkuaidi");
            Kuaidi100.GetOrAdd("广东邮政", "guangdongyouzhengwuliu");
            Kuaidi100.GetOrAdd("港中能达", "ganzhongnengda");
            Kuaidi100.GetOrAdd("凤凰", "fenghuangkuaidi");
            Kuaidi100.GetOrAdd("飞康达", "feikangda");
            Kuaidi100.GetOrAdd("fedex", "fedex");
            Kuaidi100.GetOrAdd("d速", "dsukuaidi");
            Kuaidi100.GetOrAdd("dhl", "dhl");
            Kuaidi100.GetOrAdd("dpex", "dpex");
            Kuaidi100.GetOrAdd("德邦", "debangwuliu");
            Kuaidi100.GetOrAdd("大田", "datianwuliu");
            Kuaidi100.GetOrAdd("长宇", "changyuwuliu");
            Kuaidi100.GetOrAdd("中国东方", "Coe");
            Kuaidi100.GetOrAdd("希伊艾斯", "cces");
            Kuaidi100.GetOrAdd("bht", "bht");
            Kuaidi100.GetOrAdd("彪记", "biaojikuaidi");
            Kuaidi100.GetOrAdd("百福东方", "baifudongfang");
            Kuaidi100.GetOrAdd("安信达", "anxindakuaixi");
            Kuaidi100.GetOrAdd("安捷", "anjiekuaidi");
            Kuaidi100.GetOrAdd("aae", "aae");
        }

        public static string GetCode(string express)
        {
            if (express == null)
                return null;

            foreach (var item in Kuaidi100)
            {
                if (express.StartsWith(item.Key))
                    return item.Value;
            }

            return null;
        }
    }
}
