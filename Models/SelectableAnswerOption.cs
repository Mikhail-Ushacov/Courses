using System.ComponentModel;

public class SelectableAnswerOption : AnswerOption, INotifyPropertyChanged
{
    private bool isSelected;

    public bool IsSelected
    {
        get => isSelected;
        set
        {
            isSelected = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSelected)));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}
