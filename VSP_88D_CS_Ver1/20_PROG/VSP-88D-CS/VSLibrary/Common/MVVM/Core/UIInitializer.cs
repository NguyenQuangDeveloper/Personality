using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VSLibrary.Common.MVVM.Interfaces;

namespace VSLibrary.Common.MVVM.Core;

public class UIInitializer
{
    public static void RegisterServices(IContainer container)
    {
        //#if !DEBUG && !DESIGNER
        //            // ✅ 디버그 모드(VS 실행)에서는 라이선스 체크 안 함
        //            if (!Debugger.IsAttached)
        //            {
        //                // 🔹 Rockey4ND 라이선스 검증 추가
        //                bool hasUI = LicenseManager.Instance.IsFeatureEnabled("UI");             

        //                // ✅ "Runtime"이 있으면 다른 키가 없어도 실행
        //                if (!hasUI)
        //                {
        //                    MessageBox.Show("⛔ UI 라이선스 인증 실패! 프로그램 실행 차단.");
        //                    throw new UnauthorizedAccessException("⛔ Rockey4ND 라이선스 인증 실패! 프로그램 실행 차단.");
        //                }
        //            }
        //#endif
        // View와 ViewModel 등록
        container.AutoInitialize(Assembly.GetExecutingAssembly());
    }
}
