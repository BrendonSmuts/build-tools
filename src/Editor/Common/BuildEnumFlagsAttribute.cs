using UnityEngine;


namespace SweetEngine.Build
{
    public class BuildEnumFlagsAttribute : PropertyAttribute
    {
        private int _rows;




        public int Rows
        {
            get { return _rows; }
        }




        public BuildEnumFlagsAttribute()
        {
            _rows = 1;
        }


        public BuildEnumFlagsAttribute(int rows)
        {
            _rows = Mathf.Clamp(rows, 1, 10);
        }
    }
}