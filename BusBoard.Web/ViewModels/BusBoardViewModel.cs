using System;
using BusBoard.Web.Models;

namespace BusBoard.Web.ViewModels;

public class BusBoardViewModel
{
    public string? ErrorMessage { get; set; }
    public string? Postcode { get; set; }
    public List<BusBoardEntry>? BusBoardEntries { get; set; }

    public BusBoardViewModel(string postcode, List<BusBoardEntry>? busBoardEntries = null, string? error = null)
    {
        Postcode = postcode;
        BusBoardEntries = busBoardEntries;
        ErrorMessage = error;
    }
}
