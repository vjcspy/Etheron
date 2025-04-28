using System;
using System.Collections.Generic;
using UnityEngine;
namespace ND25.Core.Utils
{
    /// <summary>
    ///     Lớp cơ sở ánh xạ từ enum sang Animator parameter hash.
    ///     Không dùng singleton, mỗi subclass cần được khởi tạo riêng.
    /// </summary>
    public abstract class AnimatorParamMap<TEnum> where TEnum : Enum
    {
        private readonly Dictionary<TEnum, int> _nameToHash = new Dictionary<TEnum, int>();
        private readonly Animator animator;

        protected AnimatorParamMap(Animator animator)
        {
            this.animator = animator;
            foreach (TEnum value in Enum.GetValues(enumType: typeof(TEnum)))
            {
                string name = value.ToString();
                int hash = Animator.StringToHash(name: name);
                _nameToHash[key: value] = hash;
            }
        }
        private int Get(TEnum param)
        {
            return _nameToHash[key: param];
        }

        public void UpdateIntParam(TEnum param, int value)
        {
            animator.SetInteger(id: Get(param: param), value: value);
        }

        public void UpdateFloatParam(TEnum param, float value)
        {
            animator.SetFloat(id: Get(param: param), value: value);
        }

        public void UpdateBoolParam(TEnum param, bool value)
        {
            animator.SetBool(id: Get(param: param), value: value);
        }
    }
}
