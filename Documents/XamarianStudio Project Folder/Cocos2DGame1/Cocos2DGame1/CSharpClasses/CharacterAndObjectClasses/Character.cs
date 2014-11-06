using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cocos2D;

namespace MindRain
{ 
	/// <summary>
	/// Note: The Character class is a data holding class that loads Character specific data and stores it.
	/// This is accomplished though loading and storing bonus stats. This class also stores all a characters base states aswell
	/// although they are loaded in the object class. Finally this class creates a subclass called fighterstate.
	/// fighterstate is what is used to store all the charaters stats using its many class variables. 
	/// A characters current fighting state can also be accessed though this subclass, this includes things like:
	/// dizzy, airborn, hit, attacking, walking, ect.
	/// </summary>
    public class Character
    {
		public fighterState state;
		public Dictionary<string, float> bonusStatsPropertyDict = new Dictionary<string, float>();
		public Object characterObj = new Object ();
		public CharacterFightingHitBoxes characterFHB = new CharacterFightingHitBoxes ();

		//Note: These two variables should be set to the name of the animation minus the character name
		public string currentAttackName = "";
		public string currentKnockBackName = "";

		public Character()
		{

		}
		public Character(Object characterObj, string bonusStats, List<string> hitboxFileNameList, CCSize animationScale)
		{
			//Note: Load bonus Stats into a dictionary
			LoadBonusStatsFiles(bonusStats, bonusStatsPropertyDict);
			this.characterObj = characterObj;

			CharacterFightingHitBoxes characterFHB = new CharacterFightingHitBoxes (hitboxFileNameList, characterObj.body.Position, animationScale);
			this.characterFHB = characterFHB;

			fighterState state = new fighterState(characterObj.propertyDict, bonusStatsPropertyDict);
			this.state = state;
		}

		private void LoadBonusStatsFiles(string bonusStats, Dictionary<string, float> bonusStatsPropertyDict)
		{
			//Read the bonusStats of the character into an array
			//Note: Read AllLines reads in by line all the content on the line
			string[] bonusS = System.IO.File.ReadAllLines("Content/propertyFiles/bonusStats/" + bonusStats + ".txt");
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
            public float healthPercentage = 0f;
			public float currentShieldHealth = 0f;

            //Note: combo length is number of sequential hits in a row includes cancels
            public int comboLength = 0;

            //Note: chance to tech only applies on teching when getting smashed into somthing
            //for example it is possible to tech a fall but since being smashed into the ground can kill you
            //there needs to be a chance to tech when being smashed into a wall or ground so a player cannot always tech out of death
            public float chanceToTech = 0f;
			public float cancelsAvalable = 0f;
			public float jumpsDone = 0f;
			public float gravityScale = 0f;


			//Note: Base value of all character stats

			//Note: This is the max range of the character it is used to see if a character should hit an enemy ie if collision detection should be done.
			public float maxRange = 0f;

			public float armor = 0f;
			public float maxShieldHealth = 0f;
			//Note: How many cancels a character gets in a row
			public int cancelCount = 0;
			public float cancelRechargeTime = 0f;

			//Note: density is used to determine weight given the gravity of the stage i think
			public float density = 0f;

			//Note: launchResistance is used for choosing how much force is going to be imparted when this character is hit
			public float launchResistance = 0f;

			//Note: restitution  is how much an object bounces when it hits somthing, 1 is conservs all energy and bounces with the same speed as originaly struck with.
			public float restitution = 0f;

			//Note: Speed may not be neccisary or may be replaced
			public float speed = 0f;
			public float dashSpeed = 0f;
			public float friction = 0f;

			//Note: max jump height works per jump
			public float maxJumpHeight = 0f;
			public float jumpLaunchSpeed = 0f;
			public float landingLag = 0f;

			//Note: how many sequental jumps a character can do
			public int numberOfJumps = 0;
			public float dodgeLength = 0f;

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
			public bool walking = false;
			public bool sprinting = false;
			public bool crouching = false;



            //Note: Character damage on all attacks total number of attacks 25
			public float neutralA = 0f;
			public float neutralB = 0f;
			public float forwardA = 0f;
			public float forwardB = 0f;
			public float upA = 0f;
			public float upB = 0f;
			public float downA = 0f;
			public float downB = 0f;

            //Note: charge rate should be in extra damage per milsec
			public float chargeRate = 0f;
			public float neutralChargeMinA = 0f;
			public float neutralChargeMaxA = 0f;
			public float neutralChargeMinB = 0f;
			public float neutralChargeMaxB = 0f;
			public float forwardChargeMinA = 0f;
			public float forwardChargeMaxA = 0f;
			public float upChargeMinA = 0f;
			public float upChargeMaxA = 0f;
			public float downChargeMinA = 0f;
			public float downChargeMaxA = 0f;
			public float airNeutralA = 0f;
			public float airForwardA = 0f;
			public float airUpA = 0f;
			public float airDownA = 0f;

			//Note: Throw damage, Midair and grounded 6 in total
			public float groundForwardThrow = 0f;
			public float groundUpThrow = 0f;
			public float groundDownThrow = 0f;
			public float airForwardThrow = 0f;
			public float airUpThrow = 0f;
			public float airDownThrow = 0f;

			//Note: each character can only take one ultra into battle but has 3 to choose from (3 is tennative could be more)
			//Note: Damage for each Ultra
			public float ultraOne = 0f;
			public float ultraTwo = 0f;
			public float ultraThree = 0f;



			//Note: Character knockback on all attacks total number of attacks 25
			public float neutralAKB = 0f;
			public float neutralBKB = 0f;
			public float forwardAKB = 0f;
			public float forwardBKB = 0f;
			public float upAKB = 0f;
			public float upBKB = 0f;
			public float downAKB = 0f;
			public float downBKB = 0f;

			//Note: charge rate should be in extra knockback per milsec
			public float chargeRateKB = 0f;
			public float neutralChargeMinAKB = 0f;
			public float neutralChargeMaxAKB = 0f;

			public float neutralChargeMinBKB = 0f;
			public float neutralChargeMaxBKB = 0f;
			public float forwardChargeMinAKB = 0f;
			public float forwardChargeMaxAKB = 0f;
			public float upChargeMinAKB = 0f;
			public float upChargeMaxAKB = 0f;
			public float downChargeMinAKB = 0f;
			public float downChargeMaxAKB = 0f;
			public float airNeutralAKB = 0f;
			public float airForwardAKB = 0f;
			public float airUpAKB = 0f;
			public float airDownAKB = 0f;

			//Note: Throw knockback, Midair and grounded 6 in total
			public float groundForwardThrowKB = 0f;
			public float groundUpThrowKB = 0f;
			public float groundDownThrowKB = 0f;
			public float airForwardThrowKB = 0f;
			public float airUpThrowKB = 0f;
			public float airDownThrowKB = 0f;

			//Note: each character can only take one ultra into battle but has 3 to choose from (3 is tennative could be more)
			//Note: knockback for each Ultra
			public float ultraOneKB = 0f;
			public float ultraTwoKB = 0f;
			public float ultraThreeKB = 0f;

			//Note: Assosiates an attack name with the damage that attack does
			public Dictionary <string, float> attackNameDict = new Dictionary<string, float>();

			//Note: Assosiates an knockback name with the amount of nockback it does
			public Dictionary <string, float> knockBackNameDict = new Dictionary<string, float>();

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
				//Note: gets the shield health and sets the current shield health to that value
				this.maxShieldHealth = characterBaseStats ["maxShieldHealth"];
				this.currentShieldHealth = this.maxShieldHealth;

				//Note: How many cancels a character gets in a row
				this.cancelCount = (int) characterBaseStats["cancelCount"];
				this.cancelRechargeTime = characterBaseStats["cancelRechargeTime"];
				//Note: density is used to determine weight given the gravity of the stage i think
				this.density = characterBaseStats["density"];

				//Note: launchResistance is used for choosing how much force is going to be imparted when this character is hit
				this.launchResistance = characterBaseStats["launchResistance"];
				this.restitution = characterBaseStats ["restitution"];

				//Note: Speed may not be neccisary or may be replaced
				this.speed = characterBaseStats["speed"];
				this.dashSpeed = characterBaseStats["dashSpeed"];
				this.gravityScale = characterBaseStats["gravityScale"];

				this.friction = characterBaseStats["friction"];
				//Note: max jump height works per jump
				this.maxJumpHeight = characterBaseStats["maxJumpHeight"];
				this.jumpLaunchSpeed = characterBaseStats ["jumpLaunchSpeed"];
				this.landingLag = characterBaseStats["landingLag"];
				//Note: how many sequental jumps a character can do
				this.numberOfJumps = (int) characterBaseStats["numberOfJumps"];
				this.dodgeLength = characterBaseStats["dodgeLength"];



				//Note: Base character stats for attacks
				//Note: Character damages on all attacks total number of attacks 25
				this.neutralA = characterBaseStats["neutralA"];
				attackNameDict.Add ("neutralA", this.neutralA);

				this.neutralB = characterBaseStats["neutralB"];
				attackNameDict.Add ("neutralB", this.neutralB);

				this.forwardA = characterBaseStats["forwardA"];
				attackNameDict.Add ("forwardA", this.forwardA);

				this.forwardB = characterBaseStats["forwardB"];
				attackNameDict.Add ("forwardB", this.forwardB);

				this.upA = characterBaseStats["upA"];
				attackNameDict.Add ("upA", this.upA);

				this.upB = characterBaseStats["upB"];
				attackNameDict.Add ("upB", this.upB);

				this.downA = characterBaseStats["downA"];
				attackNameDict.Add ("downA", this.downA);

				this.downB = characterBaseStats["downB"];
				attackNameDict.Add ("downB", this.downB);

				//Note: charge rate should be in extra damage per milsec (for all charge attacks)
				this.chargeRate = characterBaseStats["chargeRate"];

				this.neutralChargeMinA = characterBaseStats["neutralChargeMinA"];
				attackNameDict.Add ("neutralChargeMinA", this.neutralChargeMinA);

				this.neutralChargeMaxA = characterBaseStats["neutralChargeMaxA"];
				attackNameDict.Add ("neutralChargeMaxA", this.neutralChargeMaxA);

				//Note: neutral charge b is the only b with a charge
				this.neutralChargeMinB = characterBaseStats["neutralChargeMinB"];
				attackNameDict.Add ("neutralChargeMinB", this.neutralChargeMinB);

				this.neutralChargeMaxB = characterBaseStats["neutralChargeMaxB"];
				attackNameDict.Add ("neutralChargeMaxB", this.neutralChargeMaxB);

				this.forwardChargeMinA = characterBaseStats["forwardChargeMinA"];
				attackNameDict.Add ("forwardChargeMinA", this.forwardChargeMinA);

				this.forwardChargeMaxA = characterBaseStats["forwardChargeMaxA"];
				attackNameDict.Add ("forwardChargeMaxA", this.forwardChargeMaxA);

				this.upChargeMinA = characterBaseStats["upChargeMinA"];
				attackNameDict.Add ("upChargeMinA", this.upChargeMinA);

				this.upChargeMaxA = characterBaseStats["upChargeMaxA"];
				attackNameDict.Add ("upChargeMaxA", this.upChargeMaxA);

				this.downChargeMinA = characterBaseStats["downChargeMinA"];
				attackNameDict.Add ("downChargeMinA", this.downChargeMinA);

				this.downChargeMaxA = characterBaseStats["downChargeMaxA"];
				attackNameDict.Add ("downChargeMaxA", this.downChargeMaxA);

				this.airNeutralA = characterBaseStats["airNeutralA"];
				attackNameDict.Add ("airNeutralA", this.airNeutralA);

				this.airForwardA = characterBaseStats["airForwardA"];
				attackNameDict.Add ("airForwardA", this.airForwardA);

				this.airUpA = characterBaseStats["airUpA"];
				attackNameDict.Add ("airUpA", this.airUpA);

				this.airDownA = characterBaseStats["airDownA"];
				attackNameDict.Add ("airDownA", this.airDownA);

				//Note: Throws, Midair and grounded 6 in total
				this.groundForwardThrow = characterBaseStats["groundForwardThrow"];
				attackNameDict.Add ("groundForwardThrow", this.groundForwardThrow);

				this.groundUpThrow = characterBaseStats["groundUpThrow"];
				attackNameDict.Add ("groundUpThrow", this.groundUpThrow);

				this.groundDownThrow = characterBaseStats["groundDownThrow"];
				attackNameDict.Add ("groundDownThrow", this.groundDownThrow);

				this.airForwardThrow = characterBaseStats["airForwardThrow"];
				attackNameDict.Add ("airForwardThrow", this.airForwardThrow);

				this.airUpThrow = characterBaseStats["airUpThrow"];
				attackNameDict.Add ("airUpThrow", this.airUpThrow);

				this.airDownThrow = characterBaseStats["airDownThrow"];
				attackNameDict.Add ("airDownThrow", this.airDownThrow);

				//Note: each character can only take one ultra into battle but has 3 to choose from
				this.ultraOne = characterBaseStats["ultraOne"];
				attackNameDict.Add ("ultraOne", this.ultraOne);

				this.ultraTwo = characterBaseStats["ultraTwo"];
				attackNameDict.Add ("ultraTwo", this.ultraTwo);

				this.ultraThree = characterBaseStats["ultraThree"];
				attackNameDict.Add ("ultraThree", this.ultraThree);



				//Note: Character knockback on all attacks total number of attacks 25
				this.neutralAKB = characterBaseStats["neutralAKB"];
				knockBackNameDict.Add ("neutralAKB", this.neutralAKB);

				this.neutralBKB = characterBaseStats["neutralBKB"];
				knockBackNameDict.Add ("neutralBKB", this.neutralBKB);

				this.forwardAKB = characterBaseStats["forwardAKB"];
				knockBackNameDict.Add ("forwardAKB", this.forwardAKB);

				this.forwardBKB = characterBaseStats["forwardBKB"];
				knockBackNameDict.Add ("forwardBKB", this.forwardBKB);

				this.upAKB = characterBaseStats["upAKB"];
				knockBackNameDict.Add ("upAKB", this.upAKB);

				this.upBKB = characterBaseStats["upBKB"];
				knockBackNameDict.Add ("upBKB", this.upBKB);

				this.downAKB = characterBaseStats["downAKB"];
				knockBackNameDict.Add ("downAKB", this.downAKB);

				this.downBKB = characterBaseStats["downBKB"];
				knockBackNameDict.Add ("downBKB", this.downBKB);

				//Note: charge rate should be in extra knockback per milsec
				this.chargeRateKB = characterBaseStats["chargeRateKB"];
				knockBackNameDict.Add ("chargeRateKB", this.chargeRateKB);

				this.neutralChargeMinAKB = characterBaseStats["neutralChargeMinAKB"];
				knockBackNameDict.Add ("neutralChargeMinAKB", this.neutralChargeMinAKB);

				this.neutralChargeMaxAKB = characterBaseStats["neutralChargeMaxAKB"];
				knockBackNameDict.Add ("neutralChargeMaxAKB", this.neutralChargeMaxAKB);

				this.neutralChargeMinBKB = characterBaseStats["neutralChargeMinBKB"];
				knockBackNameDict.Add ("neutralChargeMinBKB", this.neutralChargeMinBKB);

				this.neutralChargeMaxBKB = characterBaseStats["neutralChargeMaxBKB"];
				knockBackNameDict.Add ("neutralChargeMaxBKB", this.neutralChargeMaxBKB);

				this.forwardChargeMinAKB = characterBaseStats["forwardChargeMinAKB"];
				knockBackNameDict.Add ("forwardChargeMinAKB", this.forwardChargeMinAKB);

				this.forwardChargeMaxAKB = characterBaseStats["forwardChargeMaxAKB"];
				knockBackNameDict.Add ("forwardChargeMaxAKB", this.forwardChargeMaxAKB);

				this.upChargeMinAKB = characterBaseStats["upChargeMinAKB"];
				knockBackNameDict.Add ("upChargeMinAKB", this.upChargeMinAKB);

				this.upChargeMaxAKB = characterBaseStats["upChargeMaxAKB"];
				knockBackNameDict.Add ("upChargeMaxAKB", this.upChargeMaxAKB);

				this.downChargeMinAKB = characterBaseStats["downChargeMinAKB"];
				knockBackNameDict.Add ("downChargeMinAKB", this.downChargeMinAKB);

				this.downChargeMaxAKB = characterBaseStats["downChargeMaxAKB"];
				knockBackNameDict.Add ("downChargeMaxAKB", this.downChargeMaxAKB);

				this.airNeutralAKB = characterBaseStats["airNeutralAKB"];
				knockBackNameDict.Add ("airNeutralAKB", this.airNeutralAKB);

				this.airForwardAKB = characterBaseStats["airForwardAKB"];
				knockBackNameDict.Add ("airForwardAKB", this.airForwardAKB);

				this.airUpAKB = characterBaseStats["airUpAKB"];
				knockBackNameDict.Add ("airUpAKB", this.airUpAKB);

				this.airDownAKB = characterBaseStats["airDownAKB"];
				knockBackNameDict.Add ("airDownAKB", this.airDownAKB);

				//Note: Throw knockback, Midair and grounded 6 in total
				this.groundForwardThrowKB = characterBaseStats["groundForwardThrowKB"];
				knockBackNameDict.Add ("groundForwardThrowKB", this.groundForwardThrowKB);

				this.groundUpThrowKB = characterBaseStats["groundUpThrowKB"];
				knockBackNameDict.Add ("groundUpThrowKB", this.groundUpThrowKB);

				this.groundDownThrowKB = characterBaseStats["groundDownThrowKB"];
				knockBackNameDict.Add ("groundDownThrowKB", this.groundDownThrowKB);

				this.airForwardThrowKB = characterBaseStats["airForwardThrowKB"];
				knockBackNameDict.Add ("airForwardThrowKB", this.airForwardThrowKB);

				this.airUpThrowKB = characterBaseStats["airUpThrowKB"];
				knockBackNameDict.Add ("airUpThrowKB", this.airUpThrowKB);

				this.airDownThrowKB = characterBaseStats["airDownThrowKB"];
				knockBackNameDict.Add ("airDownThrowKB", this.airDownThrowKB);

				//Note: each character can only take one ultra into battle but has 3 to choose from (3 is tennative could be more)
				//Note: knockback for each Ultra
				this.ultraOneKB = characterBaseStats["ultraOneKB"];
				knockBackNameDict.Add ("ultraOneKB", this.ultraOneKB);

				this.ultraTwoKB = characterBaseStats["ultraTwoKB"];
				knockBackNameDict.Add ("ultraTwoKB", this.ultraTwoKB);

				this.ultraThreeKB = characterBaseStats["ultraThreeKB"];
				knockBackNameDict.Add ("ultraThreeKB", this.ultraThreeKB);
            }

            private void addBonusStats(Dictionary<string,float> bonusStats)
            {
                if(bonusStats.Count != 0)
                {
                    //Note: Add bonus stats here
                    //Note: I have not decided what bonus stats should be allowed in game yet

                }
            }

			//Note This method adds the level up bonuses to the current character's stats
			public void LevelUpStats()
			{

			}


        }
        
    }
}
	