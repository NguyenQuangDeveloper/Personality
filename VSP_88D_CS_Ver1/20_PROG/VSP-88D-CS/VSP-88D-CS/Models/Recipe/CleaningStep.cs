using VSLibrary.Common.MVVM.ViewModels;

namespace VSP_88D_CS.Models.Recipe
{
    public class CleaningStepView : ViewModelBase
    {
        private string _title;
        public string Title
        {
            get { return _title; }
            set
            {
                SetProperty(ref _title, value);
            }

        }
        public string RFPowerValue { get; set; }
        public string Gas_1 { get; set; }
        public string Gas_2 { get; set; }
        public string Gas_3 { get; set; }
        public string Gas_4 { get; set; }
        public string Vacuum { get; set; }
        public string CleanTime { get; set; }
        public bool IsGas_1ReadOnly { get; set; } = true;
        public bool IsGas_2ReadOnly { get; set; } = true;
        public bool IsGas_3ReadOnly { get; set; } = true;
        public bool IsGas_4ReadOnly { get; set; } = true;
        public CleaningStepView()
        {

        }
        public CleaningStepView(string title)
        {
            Title = title;
            RFPowerValue = "210";
            Gas_1 = "10";
            Gas_2 = "10";
            Gas_3 = "10";
            Gas_4 = "10";
            Vacuum = "0.20";
            CleanTime = "10";
        }

        public CleaningStepView FromModel(CleaningStep cleaningStep,params bool[] isGasReadOnly)
        {
            foreach (var item in isGasReadOnly)
            {

            }
            return new CleaningStepView
            {
                Title = cleaningStep.Title,
                RFPowerValue = cleaningStep.RFPowerValue.ToString(),
                Gas_1 = cleaningStep.Gas_1.ToString(),
                Gas_2 = cleaningStep.Gas_2.ToString(),
                Gas_3 = cleaningStep.Gas_3.ToString(),
                Gas_4 = cleaningStep.Gas_4.ToString(),
                Vacuum = cleaningStep.Vacuum.ToString(),
                CleanTime = cleaningStep.CleanTime.ToString(),
                
            };
        }

        public CleaningStep ToModel()
        {
            return new CleaningStep
            {
                Title = this.Title,
                RFPowerValue = int.Parse(this.RFPowerValue),
                Gas_1 = int.Parse(Gas_1),
                Gas_2 = int.Parse(Gas_2),
                Gas_3 = int.Parse(Gas_3),
                Gas_4 = int.Parse(Gas_4),
                Vacuum = double.Parse(Vacuum),
                CleanTime = int.Parse(CleanTime),
            };
        }

    }
    public class CleaningStep
    {
        public string Title { get; set; }
        public int RFPowerValue { get; set; }
        public int Gas_1 { get; set; }
        public int Gas_2 { get; set; }
        public int Gas_3 { get; set; }
        public int Gas_4 { get; set; }
        public double Vacuum { get; set; }
        public int CleanTime { get; set; }
        public CleaningStep() { }
        public CleaningStep(string title)
        {
            Title = title;
            RFPowerValue = 210;
            Gas_1 = 10;
            Gas_2 = 10;
            Gas_3 = 10;
            Gas_4 = 10;
            Vacuum = 0.20;
            CleanTime = 10;
        }


    }


}
