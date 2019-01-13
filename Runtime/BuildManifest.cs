using System.Text;
using UnityEngine;


namespace Sweet.BuildTools
{
    public class BuildManifest : ScriptableObject
    {
        public static readonly string ManifestName = "BuildManifest";
        [SerializeField] public int m_Major = 0;
        [SerializeField] private int m_Minor = 1;
        [SerializeField] private int m_Revision = 0;
        [SerializeField] private int m_Build = 1;
        [SerializeField, HideInInspector] private string m_ScmCommitId = default(string);
        [SerializeField, HideInInspector] private string m_ScmBranch = default(string);
        [SerializeField, HideInInspector] private string m_BuildTime = default(string);
        [SerializeField, HideInInspector] private string m_BundleId = default(string);
        [SerializeField, HideInInspector] private string m_UnityVersion = default(string);




        public string ScmCommitId
        {
            get { return m_ScmCommitId; }
        }


        public string ScmBranch
        {
            get { return m_ScmBranch; }
        }


        public string BuildTime
        {
            get { return m_BuildTime; }
        }


        public string BundleId
        {
            get { return m_BundleId; }
        }


        public string UnityVersion
        {
            get { return m_UnityVersion; }
        }


        public int Major
        {
            get { return m_Major; }
        }


        public int Minor
        {
            get { return m_Minor; }
        }


        public int Revision
        {
            get { return m_Revision; }
        }


        public int Build
        {
            get { return m_Build; }
        }




        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine(string.Format("Scm Commit Id: {0}", ScmCommitId));
            sb.AppendLine(string.Format("Scm Branch: {0}", ScmBranch));
            sb.AppendLine(string.Format("Build Time: {0}", BuildTime));
            sb.AppendLine(string.Format("Bundle Id: {0}", BundleId));
            sb.AppendLine(string.Format("Unity Version: {0}", UnityVersion));
            sb.AppendLine(string.Format("Game Version: {0}.{1}.{2}", Major, Minor, Revision));
            sb.AppendLine(string.Format("Build: {0}", Build));

            return sb.ToString();
        }




        public static BuildManifest Get()
        {
            return Resources.Load<BuildManifest>(ManifestName);
        }



        public string GetShortVersionString()
        {
            return string.Format("{0}.{1}.{2}",
                Major,
                Minor,
                Revision);
        }

        public string GetVersionString()
        {
            return string.Format("{0}.{1}.{2}.{3}",
                Major,
                Minor,
                Revision,
                Build);
        }
    }
}