﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MW5.Api.Interfaces;

namespace MW5.Api.Map
{
    public interface IPrintableMap
    {
        ISpatialReference Projection { get; set; }
        IEnvelope Extents { get; }
        bool ScalebarVisible { get; set; }
        void Lock();
        bool Unlock();
        bool SnapShotToDC2(IntPtr hDC, IEnvelope extents, int width, float offsetX, float offsetY, float clipX, float clipY, float clipWidth, float clipHeight);
    }
}
