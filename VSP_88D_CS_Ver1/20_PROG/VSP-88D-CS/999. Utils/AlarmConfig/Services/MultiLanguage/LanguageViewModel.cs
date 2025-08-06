using AlarmConfig.Views;
using VSLibrary.Common.MVVM.ViewModels;

namespace AlarmConfig.Services.MultiLanguage;

public partial class LanguageViewModel : ViewModelBase
{
    private static LanguageViewModel instance;

    public static LanguageViewModel Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new LanguageViewModel();
            }
            return instance;
        }
    }

    private Dictionary<string, string> _languageResources;
    public Dictionary<string, string> LanguageResources
    {
        get => _languageResources;
        set => SetProperty(ref _languageResources, value);
    }

    public event Action LanguageResourcesUpdated;

    private LanguageViewModel()
    {
        this.LoadLanguage();
    }

    public void Refresh()
    {
        var updatedLanguage = CreateLanguage();
        LanguageResources = new Dictionary<string, string>(updatedLanguage);

        // Notify ViewModel other
        LanguageResourcesUpdated?.Invoke();
    }

    public void LoadLanguage()
    {
        LanguageResources = new Dictionary<string, string>(CreateLanguage());
    }

    private Dictionary<string, string> CreateLanguage()
    {
        Dictionary<string, string> languages = new Dictionary<string, string>();

        if (Alarm.Languages == Languages.Korean)
        {
            languages.Add("HideAlarm", "알람 숨기기");
            languages.Add("ErrorOccur", "오류 발생");
            languages.Add("Cause", "원인");
            languages.Add("Action", "조치");
            languages.Add("Confirm", "확인");
            languages.Add("AlarmMessage", "알람 메시지");
            languages.Add("SolutionMessage", "해결 메시지");
            languages.Add("AlarmCode", "알람 코드");
            languages.Add("Select", "선택");
            languages.Add("Animations", "애니메이션");
            languages.Add("Shapes", "도형");
            languages.Add("Text", "텍스트");
            languages.Add("Color", "색상");
            languages.Add("File", "파일");
            languages.Add("LineWidth", "선 두께");
            languages.Add("FontSize", "글꼴 크기");
            languages.Add("Font", "글꼴");
            languages.Add("Height", "높이");
            languages.Add("Width", "너비");
            languages.Add("Image", "이미지");
            languages.Add("Zoom", "확대/축소");
            languages.Add("Info", "정보");
            languages.Add("Warning", "경고");
            languages.Add("Question", "질문");
            languages.Add("CreateImageWithAlarmCode", "알람 코드로 이미지 생성");
            languages.Add("CreateImageWithAlarmName", "알람 이름으로 이미지 생성");
            languages.Add("msg_PleaseSelectAnAlarmCode", "알람 코드를 선택해 주세요!");
            languages.Add("msg_DoYouWantToCreate", "생성하시겠습니까?");
            languages.Add("msg_DoYouWantToSave", "저장하시겠습니까?");
            languages.Add("msg_DoYouPrepareYourImageInFolder", "폴더에 이미지를 준비하셨습니까?");
            languages.Add("msg_DoYouWantToDeleteImage", "이미지를 삭제하시겠습니까?");
            languages.Add("msg_CreateAlarmFileSuccess", "알람 파일이 성공적으로 생성되었습니다!");
            languages.Add("msg_LoadImageError", "이미지 불러오기 오류");
            languages.Add("msg_NoAvailableImage", "매핑할 수 있는 이미지가 없습니다!");
            languages.Add("msg_NoValidNameMappedImage", "매핑할 유효한 이미지가 없습니다. 이미지 파일 이름을 확인해 주십시오");
            languages.Add("msg_NoValidCodeMappedImage", "매핑할 유효한 이미지가 없습니다. 이미지 파일의 코드를 확인해 주십시오");
        }
        else if (Alarm.Languages == Languages.English)
        {
            languages.Add("HideAlarm", "Hide Alarm");
            languages.Add("ErrorOccur", "Error Occur");
            languages.Add("Cause", "Cause");
            languages.Add("Action", "Action");
            languages.Add("Confirm", "Confirm");
            languages.Add("AlarmMessage", "Alarm Message");
            languages.Add("SolutionMessage", "Solution Message");
            languages.Add("AlarmCode", "Alarm Code");
            languages.Add("Select", "Select");
            languages.Add("Animations", "Animations");
            languages.Add("Shapes", "Shapes");
            languages.Add("Text", "Text");
            languages.Add("Color", "Color");
            languages.Add("File", "File");
            languages.Add("LineWidth", "Line Width");
            languages.Add("FontSize", "Font Size");
            languages.Add("Font", "Font");
            languages.Add("Height", "Height");
            languages.Add("Width", "Width");
            languages.Add("Image", "Image");
            languages.Add("Zoom", "Zoom");
            languages.Add("Info", "Info");
            languages.Add("Warning", "Warning");
            languages.Add("Question", "Question");
            languages.Add("CreateImageWithAlarmCode", "Create image with alarm code");
            languages.Add("CreateImageWithAlarmName", "Create image with alarm name");
            languages.Add("msg_PleaseSelectAnAlarmCode", "Please select an alarm code!");
            languages.Add("msg_DoYouWantToCreate", "Do you want to create?");
            languages.Add("msg_DoYouWantToSave", "Do you want to save?");
            languages.Add("msg_DoYouWantToDeleteImage", "Do you want to delete image?");
            languages.Add("msg_CreateAlarmFileSuccess", "Create alarm file successfully!");
            languages.Add("msg_LoadImageError", "Load Image Error");
            languages.Add("msg_NoAvailableImage", "No available image for mapping!");
            languages.Add("msg_NoValidNameMappedImage", "No valid image for mapping. Please check the image file's name");
            languages.Add("msg_NoValidCodeMappedImage", "No valid image for mapping. Please check the image file's code");
        }
        else if (Alarm.Languages == Languages.Chinese)
        {
            languages.Add("HideAlarm", "隐藏警报");
            languages.Add("ErrorOccur", "发生错误");
            languages.Add("Cause", "原因");
            languages.Add("Action", "操作");
            languages.Add("Confirm", "确认");
            languages.Add("AlarmMessage", "警报信息");
            languages.Add("SolutionMessage", "解决方案信息");
            languages.Add("AlarmCode", "警报代码");
            languages.Add("Select", "选择");
            languages.Add("Animations", "动画");
            languages.Add("Shapes", "形状");
            languages.Add("Text", "文字");
            languages.Add("Color", "颜色");
            languages.Add("File", "文件");
            languages.Add("LineWidth", "线条粗细");
            languages.Add("FontSize", "字体大小");
            languages.Add("Font", "字体");
            languages.Add("Height", "高度");
            languages.Add("Width", "宽度");
            languages.Add("Image", "图像");
            languages.Add("Zoom", "缩放");
            languages.Add("Info", "信息");
            languages.Add("Warning", "警告");
            languages.Add("Question", "问题");
            languages.Add("CreateImageWithAlarmCode", "使用报警代码创建图像");
            languages.Add("CreateImageWithAlarmName", "使用报警名称创建图像");
            languages.Add("msg_PleaseSelectAnAlarmCode", "请选择一个报警代码!");
            languages.Add("msg_DoYouWantToCreate", "您要创建吗？");
            languages.Add("msg_DoYouPrepareYourImageInFolder", "您是否已在文件夹中准备好图片？");
            languages.Add("msg_DoYouWantToSave", "您要保存吗？");
            languages.Add("msg_DoYouWantToDeleteImage", "您要删除图像吗？");
            languages.Add("msg_CreateAlarmFileSuccess", "报警文件创建成功！");
            languages.Add("msg_LoadImageError", "加载图像出错");
            languages.Add("msg_NoAvailableImage", "没有可用于映射的图像!");
            languages.Add("msg_NoValidNameMappedImage", "没有可用于映射的有效图像。请检查图像文件名称");
            languages.Add("msg_NoValidCodeMappedImage", "没有可用于映射的有效图像。请检查图像文件的代码");
        }
        else if (Alarm.Languages == Languages.Vietnamese)
        {
            languages.Add("HideAlarm", "Ẩn cảnh báo");
            languages.Add("ErrorOccur", "Xảy ra lỗi");
            languages.Add("Cause", "Nguyên nhân");
            languages.Add("Action", "Hành động");
            languages.Add("Confirm", "Xác nhận");
            languages.Add("AlarmMessage", "Thông báo cảnh báo");
            languages.Add("SolutionMessage", "Thông báo giải pháp");
            languages.Add("AlarmCode", "Mã cảnh báo");
            languages.Add("Select", "Chọn");
            languages.Add("Animations", "Hiệu ứng");
            languages.Add("Shapes", "Hình vẽ");
            languages.Add("Text", "Văn bản");
            languages.Add("Color", "Màu sắc");
            languages.Add("File", "Tệp");
            languages.Add("LineWidth", "Độ dày nét vẽ");
            languages.Add("FontSize", "Cỡ chữ");
            languages.Add("Font", "Phông chữ");
            languages.Add("Height", "Chiều cao");
            languages.Add("Width", "Chiều rộng");
            languages.Add("Image", "Hình ảnh");
            languages.Add("Zoom", "Phóng to/Thu nhỏ");
            languages.Add("Info", "Thông tin");
            languages.Add("Warning", "Cảnh báo");
            languages.Add("Question", "Câu hỏi");
            languages.Add("CreateImageWithAlarmCode", "Tạo hình ảnh với mã cảnh báo");
            languages.Add("CreateImageWithAlarmName", "Tạo hình ảnh với tên cảnh báo");
            languages.Add("msg_PleaseSelectAnAlarmCode", "Vui lòng chọn một mã cảnh báo!");
            languages.Add("msg_DoYouWantToCreate", "Bạn có muốn tạo không?");
            languages.Add("msg_DoYouWantToSave", "Bạn có muốn lưu không?");
            languages.Add("msg_DoYouWantToDeleteImage", "Bạn có muốn xóa hình ảnh không?");
            languages.Add("msg_CreateAlarmFileSuccess", "Tạo file cảnh báo thành công!");
            languages.Add("msg_LoadImageError", "Lỗi tải hình ảnh");
            languages.Add("msg_NoAvailableImage", "Không có hình ảnh nào để ánh xạ!");
            languages.Add("msg_NoValidNameMappedImage", "Không có hình ảnh hợp lệ để ánh xạ. Vui lòng kiểm tra tên tệp hình ảnh");
            languages.Add("msg_NoValidCodeMappedImage", "Không có hình ảnh hợp lệ để ánh xạ. Vui lòng kiểm tra mã của tệp hình ảnh");
        }

        return languages;
    }

}
