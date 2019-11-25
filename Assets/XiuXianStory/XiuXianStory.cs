using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using LitJson;
using UnityEngine;
using UnityEngine.UI;

namespace XiuXianStory
{

    public class XiuXianStory : MonoBehaviour
    {
        public TextAsset FamilyNameAsset;
        public int Day = 1;
        //public List<Person> PersonList;
        public Dictionary<string, Person> PersonDic;
        public static XiuXianStory Instance;
        //[TextArea]
        private string infoShow;
        private Battle curCombat;
        


        private void Start()
        {
            Instance = this;
            //var commonDesire = DataManager.Instance.DesireDic["渡过飞升雷劫"];
            PersonDic = new Dictionary<string, Person>();
            PersonDic.AddNameIDItem(new Person() { NameID = "韩立", Sex = EnumSex.Male, TheLevel = DataManager.Instance.LevelDic["炼气期"] }.Initilize());
            PersonDic["韩立"].TheManaSystem.Set(100,10,20);
            int age, lifeTime;
            DataHelper.GetRandomAgeAndLifeTime(0, out age, out lifeTime);
            PersonDic["韩立"][Attr.MaxHP] = 100;
            PersonDic["韩立"][Attr.Age] = age;
            PersonDic["韩立"][Attr.LifeTime] = lifeTime;
            addCommonDesire(PersonDic["韩立"]);


            //PersonDic.AddNameIDItem(new Person() { NameID = "李寻仙", Age = 30, Sex = EnumSex.Male, TheLevel = DataManager.Instance.LevelDic["金丹期"] }.Initilize().AddRootDesire(commonDesire));
            //PersonDic["李寻仙"].TheManaSystem.Set(50, 3, 10);
            //PersonDic["李寻仙"].MaxHP = 50;

            //PersonDic.AddNameIDItem(new Person() { NameID = "张恒", Age = 18, Sex = EnumSex.Female, TheLevel = DataManager.Instance.LevelDic["筑基期"] }.Initilize().AddRootDesire(commonDesire));
            //PersonDic["张恒"].TheManaSystem.Set(10, 1, 5);
            //PersonDic["张恒"].LearnedMagicList.Add(DataManager.Instance.MagicDic["气弹术"]);
            //PersonDic["张恒"].LearnedMagicList.Add(DataManager.Instance.MagicDic["烈阳斩"]);
            //PersonDic["张恒"].MaxHP = 50;

            //PersonDic.AddNameIDItem(new Person() { NameID = "赵玉虎", Age = 12, Sex = EnumSex.Male, TheLevel = DataManager.Instance.LevelDic["炼气期"] }.Initilize().AddRootDesire(commonDesire));
            //PersonDic["赵玉虎"].TheManaSystem.Set(3, 0.5f, 1);
            //PersonDic["赵玉虎"].LearnedMagicList.Add(DataManager.Instance.MagicDic["气弹术"]);
            //PersonDic["赵玉虎"].LearnedMagicList.Add(DataManager.Instance.MagicDic["土墙术"]);
            //PersonDic["赵玉虎"].MaxHP = 3;


        }

        void addCommonDesire(Person person)
        {
            person.AddRootDesire(DataManager.Instance.DesireDic["渡过飞升雷劫"]);
            //person.AddRootDesire(new AttrDesire(AttrDesire.AttrDesireType.Add, "LifeTime", 1));
        }

        void addRandomPerson()
        {
            int tryTime = 10;
            while(tryTime > 0)
            {
                var randomName = NameCreator.GetRandomName();
                if (!PersonDic.ContainsKey(randomName))
                {
                    var tar = new Person()
                    {
                        NameID = randomName,
                        Sex = Random.Range(0, 1f) > 0.5f ? EnumSex.Male : EnumSex.Female,
                        TheLevel = DataManager.Instance.LevelDic["炼气期"]
                    }.Initilize();
                    int age, lifeTime;
                    DataHelper.GetRandomAgeAndLifeTime(0, out age, out lifeTime);
                    PersonDic.AddNameIDItem(tar);
                    addCommonDesire(tar);
                    tar.TheManaSystem.Set(3, 0.5f, 1);
                    tar.LearnedMagicList.Add(DataManager.Instance.MagicDic["气弹术"]);
                    tar.LearnedMagicList.Add(DataManager.Instance.MagicDic["土墙术"]);
                    //tar.MaxHP = 3;
                    tar[Attr.Age] = age;
                    tar[Attr.LifeTime] = lifeTime;
                    tar[Attr.MaxHP] = 3;
                    Debug.Log("AddRandomPerson "+randomName);
                    break;
                }
                tryTime--;
                if (tryTime <= 0)
                    Debug.Log("随机人物添加失败, 太多重复名称");
            }
        }

        private void Update()
        {
            if(Input.GetKey(KeyCode.Space))
            {
                updateGameLogic();
                if (showDetailHistory)
                    printStory(false);
                else
                    printStateInfo();
            }


        }

        private bool showDetailHistory = true;
        private float curMessageLevel;
        private void OnGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.BeginArea(new Rect(0, 0, Screen.width - 300, Screen.height));
            scrollPos = GUILayout.BeginScrollView(scrollPos);
            GUILayout.TextArea(infoShow);
            GUILayout.EndScrollView();
            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(Screen.width - 300, 0, 300, Screen.height));
            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("<<"))
                curMessageLevel -= 1;
            if(GUILayout.Button("<"))
                curMessageLevel -= 0.1f;
            var messageLevelInput = GUILayout.TextField(curMessageLevel.ToString());
            float.TryParse(messageLevelInput, out curMessageLevel);
            if (GUILayout.Button(">"))
                curMessageLevel += 0.1f;
            if (GUILayout.Button(">>"))
                curMessageLevel += 1f;
            curMessageLevel = Mathf.Clamp(curMessageLevel, 0, 10);

            GUILayout.EndHorizontal();

            showDetailHistory = GUILayout.Toggle(showDetailHistory, "显示详细历史");
            if (GUILayout.Button("UpdateGameLogic"))
            {
                updateGameLogic();
                if (showDetailHistory)
                    printStory(false);
                else
                    printStateInfo();
            }
            if (GUILayout.Button("AddRandomNPC"))
            {
                addRandomPerson();
            }
            if (GUILayout.Button("Print All Story"))
            {
                printStory(true);
            }
            if (GUILayout.Button("Print State Info"))
            {
                printStateInfo();
            }
            if(GUILayout.Button("开启战斗"))
            {
                var p1 = PersonDic["张恒"];
                var p2 = PersonDic["赵玉虎"];
                curCombat = BattleManager.StartCombat(p1, p2);
                infoShow = p1.Title + "向" + p2.Title + "发起挑战";

            }
            if(GUILayout.Button("战斗回合"))
            {
                if(curCombat == null)
                {
                    infoShow = "当前无战斗";
                }
                else
                {
                    curCombat.UpdateLogic();
                    infoShow = curCombat.GetCurrentMessage();
                }
            }
            if(GUILayout.Button("战斗信息(全部回合)"))
            {
                infoShow = curCombat.GetAllMessage();
            }
            GUILayout.EndVertical();
            GUILayout.EndArea();

            GUILayout.EndHorizontal();
        }

        Vector2 scrollPos;


        void updateGameLogic()
        {
            foreach (var person in PersonDic.Values)
            {
                person.UpdateGameLogic();
            }
            Day++;
        }

        void printStateInfo()
        {
            StringBuilder info = new StringBuilder();
            info.Append("当前日期" + Day+"\r\n");
            foreach (var person in PersonDic.Values)
            {
                info.Append("--------" + person.Title + "--------\r\n");
                info.Append(person.GetStateInfo());
                info.Append("\r\n");
            }
            infoShow = info.ToString();
            Debug.Log(info.ToString());
        }

        void printStory(bool all)
        {
            StringBuilder history = new StringBuilder();
            history.Append("当前日期" + Day + "----------------------------------\r\n");
            foreach (var person in PersonDic.Values)
            {
                history.Append("--------"+person.Title+"--------\r\n");
                if (all)
                    history.AppendLine(person.GetAllHistoryStr(curMessageLevel));
                else
                {
                    var story = person.GetLastStory(curMessageLevel);
                    if(story != null)
                        history.AppendLine(story.ToString());
                }
            }
            history.Append("----------------------------------\r\n");
            infoShow = history.ToString();
            Debug.Log(history.ToString());
        }

    }
}