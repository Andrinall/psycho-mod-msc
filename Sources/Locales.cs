using System.Runtime.ConstrainedExecution;
using System.Security.Cryptography;
using UnityEngine;

namespace Psycho
{
    public sealed class Locales
    {
        public static readonly string[] FRIDGE_PAPER_TEXT = new string[]
        {
            "I should take my pills\nI shouldn't be bad",
            "Я должен принимать таблетки\nЯ не должен быть плохим"
        };

        public static readonly string[] CALL_SCREMER_TEXT = new string[]
        {
            "I'm always watching you! I'm always with you! Behind you...",
            "Я всегда слежу за тобой! Я всегда с тобой! Позади тебя..."
        };

        public static readonly string[,] DEATH_PAPER = new string[,]
        {
            {
                "Man found\ndead of\nheart attack\nin region of\nAlivieska",
                "Молодой человек\nнайден умершим\nот сердечного\nприступа."
            },
            {
                "Man found\nafter committing\nsuicide in\nregion of\nAlivieska",
                "Молодой человек\nнайден умершим\nот сердечного\nприступа."
            }
        };

        public static readonly string[] DEFAULT_PAGE = new string[]
        {
            "My notes... they're gone.\nIt's all him!\nHe stole them!\nI have to find him!",
            "Мои записи... они пропали.\nЭто все он!\nОн их украл!\nЯ должен его найти!"
        };

        public static readonly string[,] FINAL_PAGE = new string[,]
        {
            { // false ending
                "My parents won't come back.\nThey left 10 years ago.\nI'm tired of waiting for them.\nI have to end all this.",
                "Родители не вернутся.\nОни уехали 10 лет назад.\nЯ устал их ждать.\nЯ должен все это закончить."
            },
            { // true ending
                "My parents sent me a postcard!\nThey are fine\nand will be home soon!",
                "Родители прислали открытку!\nС ними все хорошо\nи скоро они будут дома!"
            }
        };

        public static readonly string[,,] PAGES = new string[,,]
        {
            {
                {
                    "Today I was taken to the\nhospital. They say that I have\na headache. It's strange\nbecause my head doesn't hurt\nat all. And my mother was\nupset and cried. I don't want\nher to cry, but sometimes\nI hear her crying.",
                    "Сегодня меня водили в\nбольницу. Говорят, что\nу меня болит голова.\nЭто странно потому,\nчто она у меня совсем\nне болит. А мама\nрасстроилась и плакала.\nЯ не хочу, что бы она\nплакала, но иногда слышу,\nкак она плачет."
                },
                {
                    "I hear strange things.\nI see strange things.\nSometimes I go to a strange\nworld. I don't usually like it,\nbut today it was fun.\nThe barn was burning\nso brightly!",
                    "Я слышу странное.\r\nЯ вижу странное.\r\nИногда я перемещаюсь\r\nв странный мир.\r\nОбычно мне это не нравится,\r\nно сегодня было весело.\r\nКоровник горел так ярко!"
                }
            },
            {
                {
                    "Uncle Toivo came to visit today.\r\nMy parents say that he drinks\r\na lot and that's why his wife\r\nleft him and now there's\r\nno one to raise Penti.\r\nThey said that Penti\r\nis becoming brainless.\r\nI don't understand what\r\nthat means.",
                    "Сегодня в гости приходил\r\nдядя Тойво.\r\nРодители говорят, что он\r\nмного пьет и поэтому от\r\nнего ушла жена, и теперь\r\nнекому воспитывать Пенти.\r\nОни сказали, что Пенти\r\nстановится безголовым.\r\nНе понимаю, что это\r\nзначит."
                }, 
                {
                    "Uncle Toivo now lives alone.\r\nWhat did he do to his wife?\r\nWhy did Penty lose his head?\r\nUncle is bad.",
                    "Дядя Тойво теперь живет\r\nодин. Что он сделал со своей\r\nженой? Почему Пенти\r\nостался без головы?\r\nДядя - плохой."
                } 
            },
            { 
                {
                    "Today my grandmother took\r\nme to church.\r\nIt is a strange and boring place.",
                    "Сегодня бабушка взяла меня\r\nс собой в церковь.\r\nЭто странное и скучное\r\nместо."
                }, 
                {
                    "My grandmother took me to\r\nchurch. They said that all\r\npeople will die, that I will die,\r\nthat we are all bad and that\r\nI am bad too. I had to pray\r\nand ask for forgiveness.\r\nI DON'T WANT TO GO THERE!",
                    "Бабушка повела меня в\r\nцерковь. Там говорили, что\r\nвсе люди умрут, что я умру,\r\nчто мы все плохие и что я\r\nтоже плохой. Нужно было\r\nмолиться и просить\r\nпрощения.\r\nЯ НЕ ХОЧУ ТУДА ХОДИТЬ!"
                } 
            },
            { 
                {
                    "Uncle Yokki came to visit us\r\ntoday. Everyone had fun when\r\nhe was drunk and dancing on\r\nthe table. But grandma didn't\r\nlike it.\r\nGrandma doesn't like drunks.",
                    "Сегодня к нам в гости\r\nприходил дядя Йоуко.\r\nВсем было весело, когда он\r\nпьяный танцевал на столе.\r\nНо бабушке это не\r\nпонравилось.\r\nБабушка не любит пьяных."
                },
                {
                    "Uncle Jouko came, Grandma\r\nsaid that he would end badly\r\nand the bottle would kill him.\r\nGrandma doesn't like anyone.\r\nBut Grandma didn't like that.\r\nShe said that he would end\r\nhis life in agony and commit\r\nsuicide.\r\nGrandma is EVIL!",
                    "Приходил дядя Йоуко,\r\nбабушка сказала, что он\r\nплохо кончит и его погубит\r\nбутылка. Бабушке никто\r\nне нравится. Но бабушке это\r\nне понравилось. Она сказала,\r\nчто он закончит свою жизнь\r\nв муках и станет\r\nсамоубийцей.\r\nБабушка ЗЛАЯ!"
                } 
            },
            { 
                {
                    "The doctor said that I have\r\nbad dreams about my\r\ngrandmother because I feel\r\nguilty. I should visit her more\r\noften. I don't really like to do\r\nit because she grumbles\r\nall the time.",
                    "Доктор сказал, что я вижу\r\nплохие сны про бабушку\r\nпотому, что испытываю\r\nчувство вины. Я должен чаще\r\nездить к ней в гости.\r\nЯ не очень люблю это делать\r\nпотому, что она\r\nвсе время ворчит."
                },
                {
                    "They think grandma is good,\r\nbut she's bad! She's a witch!\r\nShe's always walking on my\r\nceiling! I see her outside my\r\nwindow!\r\nI know she's EVIL!\r\nSHE'S A WITCH!",
                    "Они думают, что бабушка\r\nхорошая, но она плохая!\r\nЭто ведьма!\r\nОна постоянно ходит\r\nпо моему потолку!\r\nЯ вижу ее за окном!\r\nЯ знаю - она ЗЛАЯ!\r\nОНА ВЕДЬМА!"
                } 
            },
            { 
                {
                    "Doctor Hollberg came.\r\nHe said he could cure me,\r\nbut I had to be obedient and\r\ntake pills. The pills would\r\nhelp me come back\r\nfrom the bad world.",
                    "Приходил доктор Холлберг.\r\nОн сказал, что может меня\r\nвылечить, но я должен быть\r\nпослушным и пить\r\nтаблетки. Они помогут мне\r\nвозвращаться из\r\nнехорошего мира."
                }, 
                {
                    "I'm forbidden to do anything!\r\nI'm not allowed to hide my\r\nhands under the blanket!\r\nI'm not allowed to go near my\r\nparents' room when their bed\r\ncreaks! If I take a shower,\r\nsomeone is always standing\r\nbehind me!\r\nAND I HAVE TO EAT\r\nTHESE DAMN PILLS!!!",
                    "Мне всё запрещают!\r\nМне нельзя прятать руки\r\nпод одеялом! Мне нельзя\r\nподходить к комнате\r\nродителей, когда у них\r\nскрипит кровать!\r\nЕсли я принимаю душ, то\r\nкто-нибудь обязательно\r\nстоит за спиной!\r\nА ЕЩЕ Я ДОЛЖЕН ЖРАТЬ\r\nЭТИ ЧЕРТОВЫ ТАБЛЕТКИ!!!"
                } 
            },
            { 
                {
                    "I am admitted to hospital.\r\nI jump at every phone call or\r\nknock on the door.\r\nMy mother says it will be\r\nbetter this way. Maybe.",
                    "Меня кладут в больницу.\r\nЯ вздрагиваю от каждого\r\nтелефонного звонка или\r\nстука в дверь. Мама говорит,\r\nчто так будет лучше.\r\nМожет быть."
                }, 
                {
                    "They decided to lock me up in\r\na mental hospital! Damn, I'm\r\nsitting here shaking.\r\nEvery phone call, every knock\r\non the door - it could be the\r\ndoctors. They say it's better\r\nthis way! Ha!\r\nThey'd be better off dead!",
                    "Они решили упрятать меня\r\nв психушку! Черт, я сижу и\r\nтрясусь.\r\nКаждый телефонный звонок,\r\nкаждый стук в дверь - это\r\nмогут быть врачи.\r\nОни говорят, что так будет\r\nлучше! Ха!\r\nЛучше бы было им умереть!"
                } 
            },
            { 
                {
                    "Today I finally came home\r\nfrom the hospital!\r\nIt was so nice to eat\r\nhome-cooked food and go for\r\na walk! The doctor said I was\r\nalmost healthy, although\r\nflare-ups could return\r\nin the future.",
                    "Сегодня я наконец-то\r\nвернулся домой из больницы!\r\nКак приятно поесть\r\nдомашней еды и погулять!\r\nДоктор сказал, что я\r\nпочти здоров, хотя\r\nобострения могут вернуться\r\nв будущем."
                }, 
                {
                    "Fuck! They finally let me go!\r\nThey forbid everything in this\r\nfucking hospital!\r\nThey'll feed me some pills\r\nand be happy.\r\nI know I'm okay.",
                    "Бля! Наконец-то меня\r\nотпустили!\r\nВ этой сраной больнице все\r\nзапрещают! Накормят\r\nтаблетками и рады.\r\nЯ-то знаю,\r\nчто со мной все в порядке."
                } 
            },
            { 
                {
                    "Today my parents went on\r\nvacation. They deserved it.\r\nMom didn't want to leave me,\r\nbut the illness has receded,\r\nmore than two years have\r\npassed, I don't think anything\r\nbad will happen.",
                    "Сегодня родители уехали в\r\nотпуск. Они это заслужили.\r\nМама не хотела меня\r\nоставлять, но болезнь\r\nотступила, прошло больше\r\nдвух лет, не думаю,\r\nчто случится что-то\r\nдурное."
                }, 
                {
                    "Finally, I'm alone!\r\nWithout this care, without this\r\nsupervision. Damn, I haven't\r\nbeen a child for a long time!\r\nI've been healthy for a long\r\ntime! I'm going to have a\r\nblast this summer!",
                    "Наконец-то я остался один!\r\nБез этой опеки, без этого\r\nнадзора. Черт, я давно уже\r\nне ребенок! Я давно уже здоров!\r\nОторвусь в это лето\r\nпо полной!"
                } 
            },
            { 
                {
                    "I remember how my father\r\nand I were messing around in\r\nthe garage, fixing the car and\r\nlistening to music. I think\r\nI can still sometimes hear\r\nhis footsteps.",
                    "Помню, как мы с отцом\r\nвозились в гараже, чинили\r\nмашину и слушали музыку.\r\nМне кажется, что я до сих\r\nпор иногда слышу,\r\nкак звучат его шаги."
                }, 
                {
                    "I accidentally broke the bottle\r\nthat dad kept in the garage.\r\nIf he finds out, he'll kill me.\r\nI hear his footsteps...\r\nHe's searching.",
                    "Я случайно разбил бутылку,\r\nкоторую папа хранил\r\nв гараже. Если он узнает,\r\nто убьет меня.\r\nЯ слышу его шаги...\r\nОн ищет меня."
                } 
            },
            { 
                {
                    "I think it's starting again.\r\nToday I woke up to the sound\r\nof dripping water, but there\r\nwas no dripping anywhere.\r\nAnd footsteps in the garage,\r\nalthough there is no one there.\r\nIs my childhood illness\r\ncoming back?",
                    "Мне кажется, что это снова\r\nначинается. Сегодня я\r\nпроснулся от звука\r\nкапающей воды, но нигде\r\nничего не капало. А еще\r\nшаги в гараже, хотя там\r\nникого нет. Неужели мое\r\nдетское заболевание\r\nвозвращается?"
                },
                {
                    "It seems I see it all again.\r\nIt's like an epiphany!\r\nAs if back then, in the\r\nhospital, they stitched up\r\nmy third eye. Never mind,\r\nI'll get back at them\r\nfor everything.",
                    "Похоже я снова вижу все это.\r\nЭто похоже на прозрение!\r\nБудто тогда, в больнице,\r\nони зашили мне третий\r\nглаз.\r\nНичего, я им за все отомщу."
                } 
            },
            { 
                {
                    "Found Dr. Hollberg's phone\r\nnumber. Unfortunately,\r\nmy pills are no longer\r\nofficially available.\r\nBut he promised to help.\r\nIt's a big risk for him,\r\nso he'll hide the medicine\r\nin different places and\r\nsend me the coordinates\r\nof these places by mail.",
                    "Нашел телефон доктора\r\nХоллберга. К сожалению\r\nмоих таблеток больше нет\r\nв официальной продаже.\r\nНо, он обещал помочь.\r\nЭто большой риск для него,\r\nпоэтому он будет прятать\r\nлекарство в разных местах\r\nи отсылать мне по почте\r\nкоординаты этих мест."
                },
                {
                    "Yes, I have regained my power!\r\nNow I know it for sure!\r\nBut for now I still need pills\r\nto return. The fool of a doctor\r\nagreed to sell them to me\r\nillegally. True, I will have to\r\nrun around, finding where\r\nhe hid the next dose.",
                    "Да, я снова обрел свою силу!\r\nТеперь знаю это наверняка!\r\nНо пока мне все еще нужны\r\nтаблетки, чтобы\r\nвозвращаться.\r\nДурак-доктор согласился\r\nнелегально продавать мне их.\r\nПравда придется побегать,\r\nотыскивая куда он\r\nприпрятал очередную дозу."
                } 
            },
            { 
                {
                    "The doctor said that in\r\naddition to taking pills,\r\nI also need psychotherapy.\r\nAs if there is a feeling of guilt\r\nhanging over me and I need to\r\nget rid of it. To do this,\r\nI need to help people, do good\r\ndeeds. Maybe this will help\r\nme overcome the disease.",
                    "Доктор сказал, что помимо\r\nприема таблеток, мне еще\r\nнужна психотерапия.\r\nБудто бы надо мной висит\r\nчувство вины и я должен\r\nизбавиться от него.\r\nДля этого нужно помогать\r\nлюдям, совершать хорошие\r\nпоступки.\r\nВозможно это поможет мне\r\nпобедить болезнь."
                },
                {
                    "It's all the fault of these\r\nbastards who want to make me\r\ntheir servant! What, Mom?\r\nNot drink alcohol?\r\nWhat, Grandma?\r\nTake you to church?\r\nWhat, Mr. Teimo?\r\nDeliver your leaflets?\r\nGO TO HELL!\r\nIT'S ALL BECAUSE OF YOU!",
                    "Всему виной эти ублюдки,\r\nкоторые хотят сделать из\r\nменя своего слугу!\r\nЧто, мама?\r\nНе пить алкоголь?\r\nЧто, бабушка?\r\nОтвезти тебя в церковь?\r\nЧто мистер Теймо?\r\nРазвезти твои листовки?\r\nКАТИТЕСЬ К ЧЕРТУ!\r\nВСЕ ИЗ-ЗА ВАС!"
                } 
            },
        };
    }
}
