using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MindRain
{
    class Character
    {
        public Dictionary<string, float> bonusStatsPropertyDict;

		public Character(Object characterObj, string fgHitboxesDef, string bonusStats)
		{
			//Note: Load bonus Stats into a dictionary
			LoadBonusStatsFiles(bonusStats, bonusStatsPropertyDict);
			fighterState state = new fighterState(characterObj.propertyDict, bonusStatsPropertyDict);
		}
		private void LoadBonusStatsFiles(string bonusStats, Dictionary<string, float> bonusStatsPropertyDict)
		{
			//Read the bonusStats of the character into an array
			//Note: Read AllLines reads in by line all the content on the line
			string[] bonusS = System.IO.File.ReadAllLines("Content/PropertyFiles/" + bonusStats + ".txt");
			foreach (string property in bonusS)
			{
				float bonusSVal = 0;
				string numberonly = "";
				string onlyLetters = "";
				//Note: This should find only the letters from each line of the file
				onlyLetters = new string(property.Where(Char.IsLetter).ToArray());
				//Note: This should find only the numbers in each line of the file
				numberonly = System.Text.RegularExpressions.Regex.Match(property, @"\d+").Value;
				bonusSVal = float.Parse(numberonly);
				//Note: This should add both the letters and the numbers from a single line of
				//the file to a dictionary for ease of access
				bonusStatsPropertyDict.Add(onlyLetters, bonusSVal);
			}

			this.bonusStatsPropertyDict = bonusStatsPropertyDict;
		}
        //Note: figher state sets all the default values to zero and they will be modified when the game is loaded
        // and as the game progresses
        public class fighterState
        {
            //Note: In Game Character values to keep track of below
            public float damage = 0f;
            //Note: combo length is number of sequential hits in a row includes cancels
            public int comboLength = 0;
            //Note: chance to tech only applies on teching when getting smashed into somthing
            //for example it is possible to tech a fall but since being smashed into the ground can kill you
            //there needs to be a chance to tech when being smashed into a wall or ground so a player cannot always tech out of death
            public float chanceToTech = 0f;
			public float cancelsAvalable = 0f;

			//Note: Base value of all character stats
			private float armor = 0f;
			//Note: How many cancels a character gets in a row
			private int cancelCount = 0;
			private float cancelRecharageTime = 0f;
			//Note: density is used to determine weight given the gravity of the stage i think
			private float density = 0f;
			//Note: Speed may not be neccisary or may be replaced
			private float speed = 0f;
			private float dashSpeed = 0f;
			private float friction = 0f;
			//Note: max jump height works per jump
			private float maxJumpHeight = 0f;
			private float jumpLaunchSpeed = 0f;
			private float landingLag = 0f;
			//Note: how many sequental jumps a character can do
			private int numberOfJumps = 0;
			private float dodgeLength = 0f;

            //Note: End of Game state screen values to keep track of
            public float totalDamageTaken = 0f;
            public float totalDamageGiven = 0f;
            public int numberOfDeaths = 0;
            public float distanceFlown = 0f;
            public int numberOfSD = 0;
            public int numberOfKills = 0;
            //Note: Match result- false is loss, true is win, everyone starts out losing until one team wins
            public bool matchResult = false;
            public float damageHealed = 0f;
            public float highestDamage = 0f;
            public int longestCombo = 0;

            //Note: Character states below
            public bool hit = false;
            public bool helpless = false;
            public bool invincible = false;
            public bool dizzy = false;
            public bool airborne = false;
            public bool attacking = false;
            public bool dead = false;
            //Note: untargetable means no interaction what so ever, think of this like a dodge where other characters move though yours
            public bool untargetable = false;
            public bool blocking = false;
            public bool teching = false;
            public bool knockedDown = false;

            //Note: Character damage on all attacks total number of attacks 25
			private float neutralA = 0f;
			private float neutralB = 0f;
			private float forwardA = 0f;
			private float forwardB = 0f;
			private float upA = 0f;
			private float upB = 0f;
			private float downA = 0f;
			private float downB = 0f;
            //Note: charge rage should be in extra damage per milsec
			private float chargeRate = 0f;
			private float neutralChargeMinA = 0f;
			private float neutralChargeMaxA = 0f;
            //Note: neutral charge b is the only b with a charge
			private float neutralChargeMinB = 0f;
			private float neutralChargeMaxB = 0f;
			private float forwardChargeMinA = 0f;
			private float forwardChargeMaxA = 0f;
			private float upChargeMinA = 0f;
			private float upChargeMaxA = 0f;
			private float downChargeMinA = 0f;
			private float downChargeMaxA = 0f;
			private float airNeutralA = 0f;
			private float airForwardA = 0f;
			private float airUpA = 0f;
			private float airDownA = 0f;

			//Note: Throw damage, Midair and grounded 6 in total
			private float groundForwardThrow = 0f;
			private float groundUpThrow = 0f;
			private float groundDownThrow = 0f;
			private float airForwardThrow = 0f;
			private float airUpThrow = 0f;
			private float airDownThrow = 0f;

			//Note: each character can only take one ultra into battle but has 3 to choose from (3 is tennative could be more)
			//Note: Damage for each Ultra
			private float ultra1 = 0f;
			private float ultra2 = 0f;
			private float ultra3 = 0f;

            //Note: Use this constructor to set all the default values of the character
            //from the dictionary created from the object properties file in object
            public fighterState(Dictionary<string,float> characterBaseStats, Dictionary<string,float> bonusStats)
            {
                instantiateCharacterStats(characterBaseStats);
                addBonusStats(bonusStats);
            }
            private void instantiateCharacterStats(Dictionary<string, float> characterBaseStats)
            {
                //Note: instantiate base stats here
                //Note: move though dict one at a time and reset the default stat

				//Note: ADD A CHECK TO FIND OUT IF EACH OF THESE STATS ARE PRESENT IN THE DICT BEFORE SETTING THE VALUES!!!!!!!

				//Note:Base Character Stats
				this.armor = characterBaseStats["armor"];
				//Note: How many cancels a character gets in a row
				this.cancelCount = (int) characterBaseStats["cancelCount"];
				this.cancelRecharageTime = characterBaseStats["cancelRechargeTime"];
				//Note: density is used to determine weight given the gravity of the stage i think
				this.density = characterBaseStats["density"];
				//Note: Speed may not be neccisary or may be replaced
				this.speed = characterBaseStats["speed"];
				this.dashSpeed = characterBaseStats["dashSpeed"];
				this.friction = characterBaseStats["friction"];
				//Note: max jump height works per jump
				this.maxJumpHeight = characterBaseStats["maxJumpHeight"];
				this.jumpLaunchSpeed = characterBaseStats ["jumpLaunchSpeed"];
				this.landingLag = characterBaseStats["landingLag"];
				//Note: how many sequental jumps a character can do
				this.numberOfJumps = (int) characterBaseStats["numberOfJumps"];
				this.dodgeLength = characterBaseStats["dodgeLength"];

				//Note: Base character stats for attacks
				//Note: Character damages on all attacks total number of attacks 23
				this.neutralA = characterBaseStats["neutralA"];
				this.neutralB = characterBaseStats["neutralB"];
				this.forwardA = characterBaseStats["forwardA"];
				this.forwardB = characterBaseStats["forwardB"];
				this.upA = characterBaseStats["upA"];
				this.upB = characterBaseStats["upB"];
				this.downA = characterBaseStats["downA"];
				this.downB = characterBaseStats["downB"];
				//Note: charge rate should be in extra damage per milsec (for all charge attacks)
				this.chargeRate = characterBaseStats["chargeRate"];
				this.neutralChargeMinA = characterBaseStats["neutralChargeMinA"];
				this.neutralChargeMaxA = characterBaseStats["neutralChargeMaxA"];
				//Note: neutral charge b is the only b with a charge
				this.neutralChargeMinB = characterBaseStats["neutralChargeMinB"];
				this.neutralChargeMaxB = characterBaseStats["neutralChargeMaxB"];
				this.forwardChargeMinA = characterBaseStats["forwardChargeMinA"];
				this.forwardChargeMaxA = characterBaseStats["forwardChargeMaxA"];
				this.upChargeMinA = characterBaseStats["upChargeMinA"];
				this.upChargeMaxA = characterBaseStats["upChargeMaxA"];
				this.downChargeMinA = characterBaseStats["downChargeMinA"];
				this.downChargeMaxA = characterBaseStats["downChargeMaxA"];
				this.airNeutralA = characterBaseStats["airNeutralA"];
				this.airForwardA = characterBaseStats["airForwardA"];
				this.airUpA = characterBaseStats["airUpA"];
				this.airDownA = characterBaseStats["airDownA"];

				//Note: Throws, Midair and grounded 6 in total
				this.groundForwardThrow = characterBaseStats["groundForwardThrow"];
				this.groundUpThrow = characterBaseStats["groundUpThrow"];
				this.groundDownThrow = characterBaseStats["groundDownThrow"];
				this.airForwardThrow = characterBaseStats["airForwardThrow"];
				this.airUpThrow = characterBaseStats["airUpThrow"];
				this.airDownThrow = characterBaseStats["airDownThrow"];

				//Note: each character can only take one ultra into battle but has 3 to choose from
				this.ultra1 = characterBaseStats["ultra1"];
				this.ultra2 = characterBaseStats["ultra2"];
				this.ultra3 = characterBaseStats["ultra3"];
            }
            private void addBonusStats(Dictionary<string,float> bonusStats)
            {
                if(bonusStats.Count != 0)
                {
                    //Note: Add bonus stats here
                    //Note: I have not decided what bonus stats should be allowed in game yet

                }
            }    
        }
        
    }
}
