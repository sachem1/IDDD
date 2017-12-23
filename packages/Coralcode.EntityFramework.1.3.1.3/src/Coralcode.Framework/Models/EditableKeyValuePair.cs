using System;

namespace Coralcode.Framework.Models
{

    // 摘要:
    //     定义可设置或检索的键/值对。
    //
    // 类型参数:
    //   TKey:
    //     键的类型。
    //
    //   TValue:
    //     值的类型。
    [Serializable]
    public struct EditableKeyValuePair<TKey, TValue>
    {
        private TKey _key;

        private TValue _value;

        //
        // 摘要:
        //     用指定的键和值初始化 System.Collections.Generic.KeyValuePair<TKey,TValue> 结构的新实例。
        //
        // 参数:
        //   key:
        //     每个键/值对中定义的对象。
        //
        //   value:
        //     与 key 相关联的定义。
        public EditableKeyValuePair(TKey key, TValue value)
        {
            _key = key;
            _value = value;
        }

        // 摘要:
        //     获取键/值对中的键。
        //
        // 返回结果:
        //     一个 TKey，它是 System.Collections.Generic.KeyValuePair<TKey,TValue> 的键。
        public TKey Key
        {
            get { return _key; }
            set { _key = value; }
        }
        //
        // 摘要:
        //     获取键/值对中的值。
        //
        // 返回结果:
        //     一个 TValue，它是 System.Collections.Generic.KeyValuePair<TKey,TValue> 的值。
        public TValue Value
        {
            get { return _value; }
            set { _value = value; }
        }
        // 摘要:
        //     使用键和值的字符串表示形式返回 System.Collections.Generic.KeyValuePair<TKey,TValue> 的字符串表示形式。
        //
        // 返回结果:
        //     System.Collections.Generic.KeyValuePair<TKey,TValue> 的字符串表示形式，它包括键和值的字符串表示形式。
        public override string ToString()
        {
            return Key + "-" + Value;
        }
    }
}
