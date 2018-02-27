using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace ProShine_Script_Creator
{
    public partial class MainWindow
    {
        private readonly List<string> _names = new List<string>();
        private readonly List<string> _maps = new List<string>();
        private readonly List<string> _abilities = new List<string>();
        private ArrayList MapsSorted = new ArrayList();
        private StringBuilder _codeBuilder;

        public MainWindow()
        {
            InitializeComponent();
            NameList.ItemsSource = _names;
            MapList.ItemsSource = _maps;
            AbilitiesList.ItemsSource = _abilities;
            Init();
        }

        private void Init()
        {
            OutputWin.AppendText("\t\t\t~ProShine Script Creator~" + Environment.NewLine);
            OutputWin.AppendText("\t\thttps://proshine-bot.com/thread-2950.html" + Environment.NewLine);

            foreach(var abiName in AbilitiesNames.AbilitiesNamesArray)
                AbilitiesName.Items.Add(abiName);

            foreach (var pokeName in PokemonNames.PokemonNamesArray)
                PokeBox.Items.Add(pokeName);

            ///add kanto maps first
            MapsSorted.Clear();
            MapsBox.Items.Clear();

            foreach (var mapName in MapNames.KantoMaps)
                MapsSorted.Add(mapName);

            MapsSorted.Sort();

            foreach (var map in MapsSorted)
                MapsBox.Items.Add(map);            

            RegionBox.Items.Add("Kanto");
            RegionBox.Items.Add("Johto");
            RegionBox.Items.Add("Hoenn");
            RegionBox.Items.Add("Sinnoh");
            RegionBox.Items.Add("Sevii Islands");
            RegionBox.Items.Add("Event Maps");

            RegionBox.SelectedIndex = 0;
            MapsBox.SelectedIndex = 0;
        }

        private void Print(string msg)
        {
            OutputWin.AppendText("PSC: " + msg + Environment.NewLine);
            OutputWin.ScrollToEnd();
        }

        //Remove pokemon from list
        private void RemovePokemonButton_Click(object sender, RoutedEventArgs e)
        {
            if (_names.Count <= 0) return;

            if (NameList.SelectedIndex == -1) return;

            _names.RemoveAt(NameList.SelectedIndex);
            NameList.Items.Refresh();
            NameList.SelectedIndex = NameList.Items.Count - 1;
        }

        //Add pokemon to list
        private void AddPokemonButton_Click(object sender, RoutedEventArgs e)
        {
            if (_names.Contains(PokeBox.Text)) return;

            _names.Add(PokeBox.Text);
            NameList.Items.Refresh();
            NameList.SelectedIndex = NameList.Items.Count - 1;
            NameList.ScrollIntoView(NameList.SelectedItem);
        }

        //Add map name to list
        private void AddMapButton_Click(object sender, RoutedEventArgs e)
        {

            if (_maps.Contains(MapsBox.Text)) return;

            _maps.Add(MapsBox.Text);
            MapList.Items.Refresh();
            MapList.SelectedIndex = MapList.Items.Count - 1;
            MapList.ScrollIntoView(MapList.SelectedItem);
        }

        //Remove map name from list
        private void RemoveMapButton_Click(object sender, RoutedEventArgs e)
        {
            if (_maps.Count <= 0) return;

            if (MapList.SelectedIndex == -1) return;

            _maps.RemoveAt(MapList.SelectedIndex);
            MapList.Items.Refresh();
            MapList.SelectedIndex = _maps.Count - 1;
        }

        private void MoveUpButton_Click(object sender, RoutedEventArgs e)
        {
            int index = MapList.SelectedIndex;

            if (index <= 0) return;

            string tmp = _maps[index - 1];
            _maps[index - 1] = _maps[index];
            _maps[index] = tmp;

            MapList.Items.Refresh();
            MapList.SelectedIndex = index - 1;
            MapList.ScrollIntoView(MapList.SelectedItem);
        }

        private void MoveDownButton_Click(object sender, RoutedEventArgs e)
        {
            int index = MapList.SelectedIndex;

            if (index == -1 || index == _maps.Count - 1) return;

            string tmp = _maps[index + 1];
            _maps[index + 1] = _maps[index];
            _maps[index] = tmp;

            MapList.Items.Refresh();
            MapList.SelectedIndex = index + 1;
            MapList.ScrollIntoView(MapList.SelectedItem);
        }

        private void WriteCode(string code, int indent = 0)
        {
            for (var i = 0; i < indent; i++)
            {
                code = "\t" + code;
            }
            _codeBuilder.AppendLine(code);
        }

        private void OnFalseSwipeChanged(object sender, RoutedEventArgs e)
        {
            Print(FalseSwipeCheckBox.IsChecked == true ? "Using False Swipe" : "Not using Flase Swipe");
        }

        private void OnAttackWildsChanged(object sender, RoutedEventArgs e)
        {
            Print(AttackWildsCheckBox.IsChecked == true ? "Attacking normal wilds" : "Fleeing from normal wilds");
        }

        private void CreditsButton_Click(object sender, RoutedEventArgs e)
        {
            Print("Credits Icesythe7, Zonz, PreciousTrainer");
        }

        //Generate the script
        private void CreateScript_Click(object sender, RoutedEventArgs e)
        {
            string valueOfRectangle = "";
            if ((Boolean)useMountCheckBox.IsChecked && string.IsNullOrEmpty(MountName.Text))
            {
                Print("Please add your mount/bicycle name in the extra text box or untick use mount/bicycle checkbox.");
                return;
            }


            if ((Boolean)RectangleMovementCheck.IsChecked && string.IsNullOrEmpty(RectangleText.Text))
            {
                Print("Rectangle movement's values can't be null or empty. Or if you don't want to add it then untick the Rectangle Movement check box");
                return;
            }
            else
            {
                valueOfRectangle = Regex.Replace(RectangleText.Text.ToLowerInvariant(), @"[^0-9, ]", "");
            }

            Print("Creating your custom script, please wait...");

            _codeBuilder = new StringBuilder();

            // Build path list
            if (_maps.Count > 1)
            {
                _codeBuilder.Append("local path = { ");
                foreach (string map in _maps)
                {
                    _codeBuilder.Append("'" + map + "', ");
                }
                WriteCode("}");
                WriteCode("");
            }

            //for those users who want to use false swipe
            if (FalseSwipeCheckBox.IsChecked == true)
            {
                WriteCode("local falseSwipePokeUser = nil");
                WriteCode("");
            }

            //for those users who want to use role play
            if(RolePlayCheckBox.IsChecked == true)
            {
                WriteCode("local rolePlayPokeUser = nil");
                WriteCode("");
                if (_abilities.Count > 0)
                {
                    WriteCode("local abilityList =");
                    WriteCode("{");
                    foreach (var abiName in _abilities)
                    {
                        WriteCode($"'{abiName}',", 1);
                    }
                    WriteCode("}");
                    WriteCode("");

                    WriteCode("");
                }
            }
            
            WriteCode("local catchList =");
            WriteCode("{");
            foreach (var name in _names)
            {
                if (name.Contains(" ")) // Mr Mime / Farfetch 'd
                    WriteCode("['" + name + "'] = true,", 1);
                else
                    WriteCode(name + " = true,", 1);
            }
            WriteCode("}");

            WriteCode("");
           
            if (_maps.Count > 1)
            {
                WriteCode("local function getPathIndex()");
                WriteCode("for i = 1, #path do", 1);
                WriteCode("if getMapName() == path[i] then", 2);
                WriteCode("return i", 3);
                WriteCode("end", 2);
                WriteCode("end", 1);
                WriteCode("fatal('error: current map is not present in the path table.')", 1);
                WriteCode("end");

                WriteCode("");

                WriteCode("local function firstMap()");
                WriteCode("if getMapName() == path[1] then", 1);
                WriteCode("return true", 2);
                WriteCode("end", 1);
                WriteCode("moveToMap(path[getPathIndex() - 1])", 1);
                WriteCode("end");

                WriteCode("");

                WriteCode("local function lastMap()");
                WriteCode("if getMapName() == path[#path] then", 1);
                WriteCode("return true", 2);
                WriteCode("end", 1);
                WriteCode("moveToMap(path[getPathIndex() + 1])", 1);
                WriteCode("end");

                WriteCode("");
            }

            //Return pokemon with false swipe or role play
            if (FalseSwipeCheckBox.IsChecked == true || RolePlayCheckBox.IsChecked == true)
            {
                WriteCode("function getPokemonWithMove(moveName)");
                WriteCode("for i = 1, getTeamSize() do", 1);
                WriteCode("if hasMove(i, moveName) then", 2);
                WriteCode("return i", 3);
                WriteCode("end", 2);
                WriteCode("end", 1);
                WriteCode("fatal('No False Swiper or Role Player could be found')", 1);
                WriteCode("return nil", 1);
                WriteCode("end");

                WriteCode("");

                WriteCode("function onStart()");

                WriteCode("foundCorrectAbi = false", 1);
                WriteCode("used_role_play = false", 1);
                WriteCode("");
                if (FalseSwipeCheckBox.IsChecked == true)
                    WriteCode("falseSwipePokeUser = getPokemonWithMove('False Swipe')", 1);
                if(RolePlayCheckBox.IsChecked == true)
                    WriteCode("rolePlayPokeUser = getPokemonWithMove('Role Play')", 1);
                WriteCode("end");

                WriteCode("");
            }

            //onPathAction()
            WriteCode("function onPathAction()");
            if (_maps.Count > 1)
            {
                if (RolePlayCheckBox.IsChecked == true)
                {
                    WriteCode("foundCorrectAbi = false", 1);
                    WriteCode("used_role_play = false", 1);
                }
                //If the false swiper got swapped.
                if (FalseSwipeCheckBox.IsChecked == true)
                    WriteCode("falseSwipePokeUser = getPokemonWithMove('False Swipe')", 1);
                //If the role player got swapped
                if (RolePlayCheckBox.IsChecked == true)
                    WriteCode("rolePlayPokeUser = getPokemonWithMove('Role Play')", 1);
                WriteCode("");

                //Using mount/bike
                if ((bool)useMountCheckBox.IsChecked)
                {
                    WriteCode($"if isOutside() and hasItem('{MountName.Text}') and not isSurfing() and not isMounted() then", 1);
                    WriteCode($"log('Using {MountName.Text}......')", 2);
                    WriteCode($"return useItem('{MountName.Text}')", 2);
                    WriteCode("end", 1);

                    WriteCode("");
                }

                if (FalseSwipeCheckBox.IsChecked == true && RolePlayCheckBox.IsChecked == true)
                    WriteCode("if isPokemonUsable(1) and isPokemonUsable(falseSwipePokeUser) and isPokemonUsable(rolePlayPokeUser) and getRemainingPowerPoints(falseSwipePokeUser, 'False Swipe') > 1 and getRemainingPowerPoints(rolePlayPokeUser, 'Role Play') > 1 then", 1);
                else if (FalseSwipeCheckBox.IsChecked == true && RolePlayCheckBox.IsChecked == false)
                    WriteCode("if isPokemonUsable(1) and isPokemonUsable(falseSwipePokeUser) and getRemainingPowerPoints(falseSwipePokeUser, 'False Swipe') > 1 then", 1);
                else if (FalseSwipeCheckBox.IsChecked == false && RolePlayCheckBox.IsChecked == true)
                    WriteCode("if isPokemonUsable(1) and isPokemonUsable(rolePlayPokeUser) and getRemainingPowerPoints(rolePlayPokeUser, 'Role Play') > 1 then", 1);
                else
                    WriteCode("if isPokemonUsable(1) then", 1);

                WriteCode("if lastMap() then", 2);
                if ((bool)RectangleMovementCheck.IsChecked)
                    WriteCode($"return moveToRectangle({valueOfRectangle})", 3);
                else
                    WriteCode("return moveToGrass() or moveToWater() or moveToNormalGround()", 3);
                WriteCode("end", 2);
                WriteCode("else", 1);
                WriteCode("if firstMap() then", 2);
                WriteCode("return usePokecenter()", 3);
                WriteCode("end", 2);
                WriteCode("end", 1);
            }
            else
            {
                if (RolePlayCheckBox.IsChecked == true)
                {
                    WriteCode("foundCorrectAbi = false", 1);
                    WriteCode("used_role_play = false", 1);
                    WriteCode("");
                }

                //Using mount/bike
                if ((bool)useMountCheckBox.IsChecked)
                {
                    WriteCode($"if isOutside() and hasItem('{MountName.Text}') and not isSurfing() and not isMounted() then", 1);
                    WriteCode($"log('Using {MountName.Text}......')", 2);
                    WriteCode($"return useItem('{MountName.Text}')", 2);
                    WriteCode("end", 1);

                    WriteCode("");
                }

                if ((bool)RectangleMovementCheck.IsChecked)
                    WriteCode($"return moveToRectangle({valueOfRectangle})", 2);
                else
                    WriteCode("return moveToGrass() or moveToWater() or moveToNormalGround()", 2);
            }
            WriteCode("end");

            WriteCode("");

            //onBattleAction()
            WriteCode("function onBattleAction()");
            if (RolePlayCheckBox.IsChecked == false)
            {
                WriteCode("if isWildBattle() and (isOpponentShiny() or getOpponentForm() ~= 0 or catchList[getOpponentName()]) then", 1);

                if (FalseSwipeCheckBox.IsChecked == true)
                {
                    WriteCode("if sendPokemon(falseSwipePokeUser) or (getOpponentHealthPercent() > 30 and useMove('False Swipe')) then", 2);
                    WriteCode("return", 3);
                    WriteCode("end", 2);
                }

                WriteCode("return useItem('Pokeball') or useItem('Great Ball') or useItem('Ultra Ball') or sendAnyPokemon()", 2);
                WriteCode("end", 1);
                if (AttackWildsCheckBox.IsChecked == true)
                    WriteCode("return attack() or sendUsablePokemon() or logout()", 1);
                else
                    WriteCode("return run() or attack() or sendAnyPokemon() or logout()", 1);
            }
            else
            {
                if (FalseSwipeCheckBox.IsChecked == true && RolePlayCheckBox.IsChecked == true)
                {
                    RolePlayLuaCode();
                }
                else if(FalseSwipeCheckBox.IsChecked == false)
                {
                    Print("Better use false swipe while using role play.");
                    return;
                }
            }
            WriteCode("end");

            WriteCode("");
            //if using role play then codes for prcessing battle messages
            if (RolePlayCheckBox.IsChecked == true)
            {
                WriteCode("function onBattleMessage(message)");
                WriteCode($"for i = 1, {_abilities.Count} do", 1);
                WriteCode("if stringContains(message, abilityList[i]) then", 2);
                WriteCode("foundCorrectAbi = true", 3);
                WriteCode("end", 2);
                WriteCode("end", 1);
                WriteCode("end");
            }
            //Copying the script
            System.Windows.Forms.Clipboard.SetText(_codeBuilder.ToString());

            Print("Script generated and copied to your clipboard.");
        }
        /// <summary>
        /// This method creates a role  play codes for the script. 
        /// </summary>
        private void RolePlayLuaCode()
        {
            WriteCode("if (isWildBattle() and (catchList[getOpponentName()])) then", 1);
            WriteCode("if isPokemonUsable(falseSwipePokeUser) and getRemainingPowerPoints(falseSwipePokeUser, 'False Swipe') ~= 0 then", 2);
            WriteCode("if not used_role_play and getActivePokemonNumber() ~= rolePlayPokeUser then", 3);
            WriteCode("return sendPokemon(rolePlayPokeUser)", 4);
            WriteCode("elseif getActivePokemonNumber() == rolePlayPokeUser and not used_role_play then", 3);
            WriteCode("used_role_play = true", 4);
            WriteCode("return useMove('Role Play')", 4);
            WriteCode("elseif foundCorrectAbi then", 3);
            WriteCode("if getActivePokemonNumber() ~= falseSwipePokeUser then", 4);
            WriteCode("return sendPokemon(falseSwipePokeUser)", 5);
            WriteCode("elseif getActivePokemonNumber() == falseSwipePokeUser and getOpponentHealth() > 1 then", 4);
            WriteCode("return weakAttack() or sendUsablePokemon() or sendAnyPokemon() or run()", 5);
            WriteCode("end", 4);
            WriteCode("end", 3);
            WriteCode("end", 2);
            WriteCode("if foundCorrectAbi or isOpponentShiny() or getOpponentForm() ~= 0 then", 2);
            WriteCode("return useItem('Pokeball') or useItem('Great Ball') or useItem('Ultra Ball') or sendAnyPokemon()", 3);
            WriteCode("end", 2);
            WriteCode("end", 1);
            if (AttackWildsCheckBox.IsChecked == true)
                WriteCode("return attack() or sendUsablePokemon() or logout()", 1);
            else
                WriteCode("return run() or attack() or sendAnyPokemon() or logout()", 1);
        }
        private void RegionBox_DropDownClosed(object sender, EventArgs e)
        {

            if (RegionBox.Text == "Kanto")
            {
                MapsSorted.Clear();
                MapsBox.Items.Clear();

                foreach (var mapName in MapNames.KantoMaps)
                    MapsSorted.Add(mapName);

                MapsSorted.Sort();

                foreach (var map in MapsSorted)
                    MapsBox.Items.Add(map);
            }

            if (RegionBox.Text == "Sevii Islands")
            {
                MapsSorted.Clear();
                MapsBox.Items.Clear();

                foreach (var mapName in MapNames.KantoMaps)
                    MapsSorted.Add(mapName);

                MapsSorted.Sort();

                foreach (var map in MapsSorted)
                    MapsBox.Items.Add(map);
            }

            if (RegionBox.Text == "Johto")
            {
                MapsSorted.Clear();
                MapsBox.Items.Clear();

                foreach (var mapName in MapNames.JohtoMaps)
                    MapsSorted.Add(mapName);

                MapsSorted.Sort();

                foreach (var map in MapsSorted)
                    MapsBox.Items.Add(map);
            }

            if (RegionBox.Text == "Hoenn")
            {
                MapsSorted.Clear();
                MapsBox.Items.Clear();

                foreach (var mapName in MapNames.HoennMaps)
                    MapsSorted.Add(mapName);

                MapsSorted.Sort();

                foreach (var map in MapsSorted)
                    MapsBox.Items.Add(map);
            }

            if (RegionBox.Text == "Sinnoh")
            {
                MapsSorted.Clear();
                MapsBox.Items.Clear();

                foreach (var mapName in MapNames.SinnohMaps)
                    MapsSorted.Add(mapName);

                MapsSorted.Sort();

                foreach (var map in MapsSorted)
                    MapsBox.Items.Add(map);
            }

            if (RegionBox.Text == "Event Maps")
            {
                MapsSorted.Clear();
                MapsBox.Items.Clear();

                foreach (var mapName in MapNames.EventMaps)
                    MapsSorted.Add(mapName);

                MapsSorted.Sort();

                foreach (var map in MapsSorted)
                    MapsBox.Items.Add(map);
            }

            MapsBox.SelectedIndex = 0;
        }

        private void RolePlayCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Print("Using Role Play");
        }

        private void RolePlayCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            Print("Not using Role Play");
        }

        private void RemoveAbilityButton_Click(object sender, RoutedEventArgs e)
        {
            if (_abilities.Count <= 0) return;

            if (AbilitiesList.SelectedIndex == -1) return;

            _abilities.RemoveAt(AbilitiesList.SelectedIndex);
            AbilitiesList.Items.Refresh();
            AbilitiesList.SelectedIndex = _abilities.Count - 1;
        }

        private void AddAbilityButton_Click(object sender, RoutedEventArgs e)
        {
            if (_abilities.Contains(AbilitiesName.Text)) return;

            _abilities.Add(AbilitiesName.Text);
            AbilitiesList.Items.Refresh();
            AbilitiesList.SelectedIndex = AbilitiesList.Items.Count - 1;
            AbilitiesList.ScrollIntoView(AbilitiesList.SelectedItem);
        }

        private void RectangleMovementCheck_Checked(object sender, RoutedEventArgs e)
        {
            RectangleText.Visibility = Visibility.Visible;
        }

        private void RectangleMovementCheck_Unchecked(object sender, RoutedEventArgs e)
        {
            RectangleText.Visibility = Visibility.Hidden;
        }

        private void useMountCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            MountName.Visibility = Visibility.Hidden;
            Height = 367.741;
            CreateScript.Margin = new Thickness(394, 296, 0, 0);
        }

        private void useMountCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            MountName.Visibility = Visibility.Visible;
            Height = 396.984;
            CreateScript.Margin = new Thickness(394, 323, 0, 0);
        }
    }
}
