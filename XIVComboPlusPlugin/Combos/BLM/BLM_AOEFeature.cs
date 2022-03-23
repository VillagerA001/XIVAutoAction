//using Dalamud.Game.ClientState.JobGauge.Types;
//using XIVComboPlus;
//using XIVComboPlus.Combos;

//namespace XIVComboPlus.Combos.BLM;

//internal class BLM_AOEFeature : BLMCombo
//{
//    public override string ComboFancyName => "群体GCD";

//    public override string Description => "替换火2非常牛逼的群攻GCD。";

//    protected internal override uint[] ActionIDs => new uint[] { Actions.Fire2.ActionID };

//    protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
//    {
//        uint act;
//        if (IsMoving)
//        {
//            //如果在移动并且有目标。
//            if (HaveValidTarget)
//            {
//                if (level >= Actions.Xenoglossy.Level &&  Actions.Flare.TryUseAction(level, out act)) return act;
//                if (Actions.Triplecast.TryUseAction(level, out act)) return act;
//                if (GeneralActions.Swiftcast.TryUseAction(level, out act)) return act;
//            }
//            //如果在移动，但是没有目标。
//            else
//            {
//                if (Actions.UmbralSoul.TryUseAction(level, out act))
//                {
//                    if (level < Actions.Paradox.Level)
//                    {
//                        return act;
//                    }
//                    else
//                    {
//                        if (JobGauge.UmbralIceStacks > 2 && JobGauge.UmbralHearts > 2)
//                        {
//                            return act;
//                        }
//                    }
//                }
//                return Actions.Transpose.ActionID;
//            }
//        }

//        if (CanAddAbility(level, out act)) return act;
//        if (MantainceState(level, lastComboActionID, out act)) return act;
//        if (AttackAndExchange(level, out act)) return act;
//        return actionID;
//    }

//    private bool AttackAndExchange(byte level, out uint act)
//    {
//        if (JobGauge.InUmbralIce)
//        {
//            if (Actions.Freeze.TryUseAction(level, out act)) return true;
//            if (Actions.Blizzard2.TryUseAction(level, out act)) return true;
//        }
//        else if (JobGauge.InAstralFire)
//        {
//            //如果没蓝了，就直接冰状态。
//            if (Service.ClientState.LocalPlayer.CurrentMp == 0)
//            {
//                if (AddUmbralIceStacks(level, out act)) return true;
//            }

//            //如果通晓满了，就放掉。
//            if (IsPolyglotStacksFull)
//            {
//                if (Actions.Foul.TryUseAction(level, out act)) return true;
//            }

//            //如果蓝不够了，赶紧一个绝望。
//            if (level >= 58 && JobGauge.UmbralHearts < 2)
//            {
//                if (Actions.Flare.TryUseAction(level, out act)) return true;
//            }

//            //试试看火2
//            if (Actions.Fire2.TryUseAction(level, out act)) return true;

//            //再试试看绝望
//            if (Actions.Flare.TryUseAction(level, out act)) return true;

//            //啥都放不了的话，转入冰状态。
//            if (AddUmbralIceStacks(level, out act)) return true;
//        }

//        act = 0;
//        return false;
//    }


//    /// <summary>
//    /// 保证冰火都是最大档数，保证有雷，如果条件允许，赶紧转火。
//    /// </summary>
//    /// <param name="level"></param>
//    /// <param name="lastAct"></param>
//    /// <param name="act"></param>
//    /// <returns></returns>
//    private bool MantainceState(byte level, uint lastAct, out uint act)
//    {
//        if (JobGauge.InUmbralIce)
//        {
//            if (HaveEnoughMP && (JobGauge.UmbralHearts == 3 || level < 58))
//            {
//                if (AddAstralFireStacks(level, out act)) return true;
//            }

//            if (AddUmbralIceStacks(level, out act)) return true;
//            if (AddUmbralHeartsArea(level, out act)) return true;
//            if (AddThunderArea(level, lastAct, out act)) return true;
//        }
//        else if (JobGauge.InAstralFire)
//        {
//            if (AddAstralFireStacks(level, out act)) return true;
//            if (AddThunderArea(level, lastAct, out act)) return true;
//        }
//        else
//        {
//            //没状态，就加个冰状态。
//            if (AddUmbralIceStacks(level, out act)) return true;
//        }

//        return false;
//    }

//    private bool AddUmbralIceStacks(byte level, out uint act)
//    {
//        //如果冰满了，就别加了。
//        act = 0;
//        if (JobGauge.UmbralIceStacks > 2 && JobGauge.ElementTimeRemaining > 3000) return false;

//        //试试看冰2
//        if (Actions.Blizzard2.TryUseAction(level, out act)) return true;

//        return false;
//    }

//    private bool AddAstralFireStacks(byte level, out uint act)
//    {
//        //如果火满了，就别加了。
//        act = 0;
//        if (JobGauge.AstralFireStacks > 2 && JobGauge.ElementTimeRemaining > 3000) return false;

//        if (Service.ClientState.LocalPlayer.CurrentMp < 5000)
//        {
//            if (AddUmbralIceStacks(level, out act)) return true;
//        }

//        //试试看火2
//        if (Actions.Fire2.TryUseAction(level, out act)) return true;

//        return false;
//    }

//    private bool AddThunderArea(byte level, uint lastAct, out uint act)
//    {
//        //试试看雷2
//        if (Actions.Thunder2.TryUseAction(level, out act, lastAct)) return true;

//        return false;
//    }

//    private bool AddUmbralHeartsArea(byte level, out uint act)
//    {
//        //如果满了，或者等级太低，没有冰心，就别加了。
//        act = 0;
//        if (JobGauge.UmbralHearts == 3 || level < 58) return false;

//        //冻结
//        if (Actions.Freeze.TryUseAction(level, out act)) return true;

//        return false;
//    }
//}
