using VSLibrary.Database;

namespace VSLibrary.Common.MVVM.Models
{
    /// <summary>
    /// ErrDefine 테이블의 각 레코드를 나타내는 클래스입니다.
    /// </summary>
    public class ErrorItem
    {
        /// <summary>
        /// 에러 코드입니다.
        /// </summary>
        [PrimaryKey]
        public int Code { get; set; }

        /// <summary>
        /// 에러 이름입니다.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 에러 원인입니다.
        /// </summary>
        public string Cause { get; set; }

        /// <summary>
        /// 에러 해결 방법입니다.
        /// </summary>
        public string Action { get; set; }

        /// <summary>
        /// 영어로 된 에러 이름입니다.
        /// </summary>
        public string Name_E { get; set; }

        /// <summary>
        /// 영어로 된 에러 원인입니다.
        /// </summary>
        public string Cause_E { get; set; }

        /// <summary>
        /// 영어로 된 에러 해결 방법입니다.
        /// </summary>
        public string Action_E { get; set; }

        /// <summary>
        /// 중국어로 된 에러 이름입니다.
        /// </summary>
        public string Name_C { get; set; }

        /// <summary>
        /// 중국어로 된 에러 원인입니다.
        /// </summary>
        public string Cause_C { get; set; }

        /// <summary>
        /// 중국어로 된 에러 해결 방법입니다.
        /// </summary>
        public string Action_C { get; set; }

        /// <summary>
        /// 베트남어로 된 에러 이름입니다.
        /// </summary>
        public string Name_V { get; set; }

        /// <summary>
        /// 베트남어로 된 에러 원인입니다.
        /// </summary>
        public string Cause_V { get; set; }

        /// <summary>
        /// 베트남어로 된 에러 해결 방법입니다.
        /// </summary>
        public string Action_V { get; set; }
    }
}
