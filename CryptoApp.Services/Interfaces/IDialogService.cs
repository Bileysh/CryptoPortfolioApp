namespace CryptoApp.Services.Interfaces;

public interface IDialogService
{
    void ShowMessage(string message, string title);
    void ShowError(string message, string title);
}