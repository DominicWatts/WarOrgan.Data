using System.Text.RegularExpressions;
using WarOrgan.Model.Faction;

namespace WarOrgan.Data.BuildDataset.Utils;

public static partial class RuleTextUtils
{
    public static DatasetBuildModel NormalizeRuleText(this DatasetBuildModel dataset)
    {
        foreach (var stratagem in dataset.Definition.CoreStratagems)
            NormalizeStratagem(stratagem);

        foreach (var entry in dataset.Rosters)
        {
            var roster = entry.Roster;
            foreach (var info in roster.ArmyRules.Concat(roster.Units.SelectMany(unit => unit.UnitAbilities.Concat(unit.Infos))))
                info.Text = ToPlainText(info.Text);

            foreach (var detachment in roster.Detachments)
            {
                if (detachment.Rule is { } rule)
                    rule.Text = ToPlainText(rule.Text);

                foreach (var stratagem in detachment.Stratagems)
                    NormalizeStratagem(stratagem);

                foreach (var enhancement in detachment.Enhancements)
                    if (enhancement.Description is { } description)
                        enhancement.Description = ToPlainText(description);
            }
        }

        return dataset;
    }

    private static void NormalizeStratagem(Stratagem stratagem)
    {
        stratagem.When = ToPlainText(stratagem.When);
        stratagem.Effect = ToPlainText(stratagem.Effect);
        if (stratagem.Target is { } target)
            stratagem.Target = ToPlainText(target);
        if (stratagem.Restrictions is { } restrictions)
            stratagem.Restrictions = ToPlainText(restrictions);
        if (stratagem.Options is { } options)
            foreach (var option in options)
                option.Text = ToPlainText(option.Text);
    }

    private static string ToPlainText(string text)
    {
        if (!RuleTag().IsMatch(text))
            return text;

        return RuleTag().Replace(text, match => match.Value.ToLowerInvariant() switch
        {
            "<li>" => "\n- ",
            "<br>" or "<br/>" or "<br />" or "</ul>" or "</ol>" => "\n",
            _ => string.Empty
        }).Trim();
    }

    [GeneratedRegex(@"</?(?:b|i|ins|k|u|ul|ol|li)>|<br\s*/?>", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex RuleTag();
}
