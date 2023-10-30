using cAlgo.API;

namespace cAlgo.Robots.Helpers;

internal class Orders
{
    private readonly Robot _myBot;
    private readonly string _strategyName;

    public Orders(Robot myBot, string strategyName)
    {
        _myBot = myBot;
        _strategyName = strategyName;
        _myBot.Positions.Opened += PositionsOnOpened;
    }
    
    public void ExecuteMarketOrder(TradeType tradeType, string symbolName, double amount, int stopLoss)
    {
        _myBot.ExecuteMarketOrderAsync(tradeType, symbolName, amount, _strategyName, stopLoss, null);
    }
    
    private void PositionsOnOpened(PositionOpenedEventArgs args)
    {
        _myBot.Print("{0} - {1} position opened at price {2}", _strategyName, args.Position.TradeType == TradeType.Buy ? "Long" : "Short", args.Position.EntryPrice);
    }
    
    public void ClosePosition(TradeType tradeType)
    {
        var positions = _myBot.Positions.FindAll(_strategyName, _myBot.SymbolName, tradeType);

        foreach (var position in positions)
        {
            _myBot.ClosePositionAsync(position);
        }
    }
}
