using System;
using BusBoard.Web.Models;

namespace BusBoard.Web.ViewModels;

public class BusBoardViewModel
{
    public string? Postcode { get; set; }
    public BusBoardEntry[]? BusBoardEntries { get; set; }

    public BusBoardViewModel(string? postcode, BusBoardEntry[]? busBoardEntries)
    {
        Postcode = postcode;
        BusBoardEntries = busBoardEntries;
    }
}
