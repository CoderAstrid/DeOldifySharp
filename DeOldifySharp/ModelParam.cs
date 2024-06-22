using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeOldifySharp
{
    public enum ModelType{
        eArtist,
        eStable
    }

    public class ModelParam
    {
        public bool half
        {
            get;
            set;
        }
        public ModelType modelType
        {
            get; set;
        }

        public bool simd
        {
            get; set;
        }

        public ModelParam(bool _half = true, ModelType _type = ModelType.eArtist, bool _simd = true)
        {
            half = _half;
            modelType = _type;
            simd = _simd;
        }
    }
}
