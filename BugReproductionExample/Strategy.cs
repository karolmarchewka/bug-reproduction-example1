using System.Linq;
using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.Robots.Helpers;

namespace cAlgo.Robots;

internal class Strategy
{
    private readonly Robot _myBot;
    private string _strategyName;
    private readonly Bars _renko1Pips;
    private readonly Bars _renko2Pips;
    private readonly Bars _renko3Pips;
    private MacdHistogram _macd0;
    private MacdHistogram _macd1;
    private MacdHistogram _macd2;
    private MacdHistogram _macd3;
    private Orders _order;
    private bool _firstConditionsForOpenLongOrder;
    private bool _secondConditionsForOpenLongOrder;
    private bool _firstConditionsForCloseLongPosition;
    private bool _secondConditionsForCloseLongPosition;
    private bool _allowOpenLongOrder = true;

    public Strategy(Robot myBot, string strategyName, Bars renko1Pips, Bars renko2Pips, Bars renko3Pips,
        MacdHistogram macd0, MacdHistogram macd1, MacdHistogram macd2,
        MacdHistogram macd3)
    {
        _myBot = myBot;
        _strategyName = strategyName;
        _renko1Pips = renko1Pips;
        _renko2Pips = renko2Pips;
        _renko3Pips = renko3Pips;
        _macd0 = macd0;
        _macd1 = macd1;
        _macd2 = macd2;
        _macd3 = macd3;
        _order = new Orders(_myBot, _strategyName);
    }

    public void Execute()
    {
        CheckConditions();
        ModifyOrders();
        CloseOrders();
        OpenOrders();
        ValidateConditions();
    }

    private void CheckConditions()
    {
        CheckConditionsForOpenLongOrder();
    }

    private void CheckConditionsForOpenLongOrder()
    {
        CheckFirstConditionForLongOrder();
        CheckSecondConditionForLongOrder();
    }

    private void CheckFirstConditionForLongOrder()
    {
        if (!_firstConditionsForOpenLongOrder)
        {
            _firstConditionsForOpenLongOrder = _macd0.Histogram.Last(1) >= 0 &&
                                               _macd1.Histogram.Last(1) < 0 &&
                                               _macd1.Histogram.Last(2) >= 0 &&
                                               _macd2.Histogram.Last(1) < 0 &&
                                               _macd3.Histogram.Last(1) < 0;
            AllowLongOrderOpening(true);
        }
    }

    private void CheckSecondConditionForLongOrder()
    {
        if (_firstConditionsForOpenLongOrder)
        {
            _secondConditionsForOpenLongOrder = _macd3.Histogram.Last(1) > 0 &&
                                                _macd3.Histogram.Last(2) >= 0;
        }
    }

    private void CloseOrders()
    {
        CloseLongOrders();
        CloseShortOrders();
    }

    private void CloseLongOrders()
    {
        if (CanCloseLongOrder())
        {
            _order.ClosePosition(TradeType.Buy);
            ResetToDefaultValuesWhenLongPositionsClosed();
        }
    }

    private void ResetToDefaultValuesWhenLongPositionsClosed()
    {
        _allowOpenLongOrder = true;
        _firstConditionsForCloseLongPosition = false;
        _secondConditionsForCloseLongPosition = false;
    }

    private bool CanCloseLongOrder()
    {
        return IsLongOrderOpened() && AreConditionsForClosingLongPositionValid();
    }

    private bool AreConditionsForClosingLongPositionValid()
    {
        return _firstConditionsForCloseLongPosition && _secondConditionsForCloseLongPosition;
    }

    private bool IsLongOrderOpened()
    {
        return _myBot.Positions.Count(p =>
            p.TradeType == TradeType.Buy && p.SymbolName == _myBot.SymbolName && p.Label == _strategyName) > 0;
    }

    private void CloseShortOrders()
    {
    }

    private void ModifyOrders()
    {
    }

    private void OpenOrders()
    {
        OpenLongOrder();
        OpenShortOrder();
    }

    private void OpenLongOrder()
    {
        if (CanOpenLongOrder())
        {
            var volumeInUnits = CalculateLotSize(0.1);
            _order.ExecuteMarketOrder(TradeType.Buy, _myBot.SymbolName, volumeInUnits, 5);
            AllowLongOrderOpening(false);
        }
    }

    private double CalculateLotSize(double lotSize)
    {
        return _myBot.Symbol.QuantityToVolumeInUnits(lotSize);
    }

    private bool CanOpenLongOrder()
    {
        return _allowOpenLongOrder && _firstConditionsForOpenLongOrder && _secondConditionsForOpenLongOrder;
    }

    private void AllowLongOrderOpening(bool allow)
    {
        _allowOpenLongOrder = allow;
    }

    private void OpenShortOrder()
    {
    }

    private void ValidateConditions()
    {
        ValidateConditionsForOrdersOpening();
        ValidateConditionsForClosingPosition();
    }

    private void ValidateConditionsForOrdersOpening()
    {
        ValidateConditionsForOpeningLongOrders();
        ValidateConditionsForOpeningShortOrders();
    }

    private void ValidateConditionsForOpeningLongOrders()
    {
        ValidateFirstConditionsForOpeningLongOrder();
        ValidateSecondConditionsForOpeningLongOrder();
    }

    private void ValidateFirstConditionsForOpeningLongOrder()
    {
        if (_firstConditionsForOpenLongOrder)
        {
            _firstConditionsForOpenLongOrder = !(_macd0.Histogram.Last(1) < 0 || _macd1.Histogram.Last(1) > 0 ||
                                                 _macd2.Histogram.Last(1) > 0);
        }
    }

    private void ValidateSecondConditionsForOpeningLongOrder()
    {
        if (_firstConditionsForOpenLongOrder)
        {
            _secondConditionsForOpenLongOrder = !(_macd3.Histogram.Last(1) < 0);
        }
    }

    private void ValidateConditionsForOpeningShortOrders()
    {
    }

    private void ValidateConditionsForClosingPosition()
    {
        ValidateConditionsForClosingLongPosition();
        ValidateConditionsForClosingShortPosition();
    }


    private void ValidateConditionsForClosingLongPosition()
    {
        ValidateFirstConditionForClosingLongPosition();
        ValidateSecondConditionForClosingLongPosition();
    }

    private void ValidateFirstConditionForClosingLongPosition()
    {
        var arePositionsOpened = CheckOpenedPositions(TradeType.Buy);
        if (arePositionsOpened && !_firstConditionsForCloseLongPosition)
        {
            _firstConditionsForCloseLongPosition = _macd2.Histogram.Last(1) > 0;
        }
    }

    private void ValidateSecondConditionForClosingLongPosition()
    {
        if (_firstConditionsForCloseLongPosition)
        {
            _secondConditionsForCloseLongPosition = _macd2.Histogram.Last(1) >= 0;
        }
    }
    
    private bool CheckOpenedPositions(TradeType tradeType)
    {
        return _myBot.Positions.FindAll(_strategyName, _myBot.SymbolName, tradeType).Any();
    }


    private void ValidateConditionsForClosingShortPosition()
    {
    }
}
