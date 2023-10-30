using System;
using cAlgo.API.Internals;

namespace cAlgo.Robots.Helpers;

public static class SymbolExtensions
{
    /// <summary>
    /// Returns a symbol pip value
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns>double</returns>
    public static double GetPip(this Symbol symbol)
    {
        return symbol.TickSize / symbol.PipSize * Math.Pow(10, symbol.Digits);
    }

    /// <summary>
    /// Returns a price value in terms of pips
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="price">The price level</param>
    /// <returns>double</returns>
    public static double ToPips(this Symbol symbol, double price)
    {
        return price * symbol.GetPip();
    }

    /// <summary>
    /// Returns a price value in terms of ticks
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="price">The price level</param>
    /// <returns>double</returns>
    public static double ToTicks(this Symbol symbol, double price)
    {
        return price * Math.Pow(10, symbol.Digits);
    }

    /// <summary>
    /// Rounds a price level to the number of symbol digits
    /// </summary>
    /// <param name="symbol">The symbol</param>
    /// <param name="price">The price level</param>
    /// <returns>double</returns>
    public static double Round(this Symbol symbol, double price)
    {
        return Math.Round(price, symbol.Digits);
    }

    /// <summary>
    /// Normalize x Pips amount decimal places to something that can be used as a stop loss or take profit for an order.
    /// For example if symbol is EURUSD and you pass to this method 10.456775 it will return back 10.5
    /// </summary>
    /// <param name="symbol">The symbol</param>
    /// <param name="pips">The amount of Pips</param>
    /// <returns>double</returns>
    public static double NormalizePips(this Symbol symbol, double pips)
    {
        var currentPrice = Convert.ToDecimal(symbol.Bid);

        var pipSize = Convert.ToDecimal(symbol.PipSize);

        var pipsDecimal = Convert.ToDecimal(pips);

        var pipsAddedToCurrentPrice = Math.Round((pipsDecimal * pipSize) + currentPrice, symbol.Digits);

        var tickSize = Convert.ToDecimal(symbol.TickSize);

        var result = (pipsAddedToCurrentPrice - currentPrice) * tickSize / pipSize *
                     Convert.ToDecimal(Math.Pow(10, symbol.Digits));

        return decimal.ToDouble(result);
    }
}
