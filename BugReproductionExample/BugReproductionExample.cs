using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cAlgo.API;
using cAlgo.API.Collections;
using cAlgo.API.Indicators;
using cAlgo.API.Internals;
using cAlgo.Robots.Helpers;

namespace cAlgo.Robots
{
    [Robot(AccessRights = AccessRights.None)]
    public class BugReproductionExample : Robot
    {
        private Bars _renko1Pips;
        private Bars _renko2Pips;
        private Bars _renko3Pips;
        
        private MacdHistogram _macd2Renko1;
        private MacdHistogram _macd2Renko2;
        private MacdHistogram _macd2Renko3;
        
        private MacdHistogram _macd3Renko1;
        
        private Strategy _strategy2;

        protected override void OnStart()
        {
            DataInitialization();
            IndicatorsInitialization();
            StrategyInitialization();
        }

        protected override void OnTick()
        {
            _strategy2.Execute();
        }

        protected override void OnStop()
        {
        }
        
        private void DataInitialization()
        {
            _renko1Pips = MarketData.GetBars(TimeFrame.Renko1);
            _renko2Pips = MarketData.GetBars(TimeFrame.Renko2);
            _renko3Pips = MarketData.GetBars(TimeFrame.Renko3);
        }
        
        private void IndicatorsInitialization()
        {
            _macd2Renko1 = Indicators.MacdHistogram(_renko3Pips.ClosePrices, 8, 4, 2);
            _macd2Renko2 = Indicators.MacdHistogram(_renko2Pips.ClosePrices, 8, 4, 2);
            _macd2Renko3 = Indicators.MacdHistogram(_renko1Pips.ClosePrices, 8, 4, 2);
            
            _macd3Renko1 = Indicators.MacdHistogram(_renko3Pips.ClosePrices, 12, 6, 3);

        }
        
        private void StrategyInitialization()
        {
            _strategy2 = new Strategy(this, StrategyName.Strategy2, _renko1Pips, _renko2Pips, _renko3Pips, _macd3Renko1, _macd2Renko1, _macd2Renko2, _macd2Renko3);
        }
    }
}
