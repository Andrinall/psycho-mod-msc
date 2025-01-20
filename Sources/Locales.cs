
namespace Psycho
{
    class Locales
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
                "Парень\nнайден\nумершим от\nсердечного\nприступа."
            },
            {
                "Man found\nafter committing\nsuicide in\nregion of\nAlivieska",
                "Парень\nнайден\nмёртвым на\nЖ/Д путях."
            }
        };

        public static readonly string[,] BUY_ITEMS = new string[,]
        {
            { "Money first!", "Деньги вперёд!" },
            { "You can find this in your home", "Найдёшь это у себя дома." }
        };


        public static readonly string[] POSTCARD_TEXT = new string[]
        {
            "Son! We hope\nyou're doing well!\nWe'll be back soon,\ndon't be bored!",
            "Сынок! Надеемся,\nчто у тебя все\nхорошо!\nСкоро вернемся,\nне скучай!"
        };


        public static readonly string[] DEFAULT_PAGE = new string[]
        {
            "My notes... they're gone.\nIt's all him!\nHe stole them!\nI have to find him!",
            "Мои записи... они пропали.\nЭто все он!\nОн их украл!\nЯ должен его найти!"
        };

        // [idx, (0 - true, 1 - false), lang]
        public static readonly string[,] FINAL_PAGE = new string[,]
        {
            { // true ending
                "My parents sent me a postcard!\nThey are fine\nand will be home soon!",
                "Родители\nприслали открытку!\nС ними все хорошо\nи скоро они будут дома!"
            },
            { // false ending
                "My parents won't come back.\nThey left 10 years ago.\nI'm tired of waiting for them.\nI have to end all this.",
                "Родители не вернутся.\nОни уехали 10 лет назад.\nЯ устал их ждать.\nЯ должен все это закончить."
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
                    "Я слышу странное.\nЯ вижу странное.\nИногда я перемещаюсь\nв странный мир.\nОбычно мне это не нравится,\nно сегодня было весело.\nКоровник горел так ярко!"
                }
            },
            {
                {
                    "Uncle Toivo came to visit today.\nMy parents say that he drinks\na lot and that's why his wife\nleft him and now there's\nno one to raise Penti.\nThey said that Penti\nis becoming brainless.\nI don't understand what\nthat means.",
                    "Сегодня в гости приходил\nдядя Тойво.\nРодители говорят, что он\nмного пьет и поэтому от\nнего ушла жена, и теперь\nнекому воспитывать Пенти.\nОни сказали, что Пенти\nстановится безголовым.\nНе понимаю, что это\nзначит."
                }, 
                {
                    "Uncle Toivo now lives alone.\nWhat did he do to his wife?\nWhy did Penty lose his head?\nUncle is bad.",
                    "Дядя Тойво теперь живет\nодин. Что он сделал со своей\nженой? Почему Пенти\nостался без головы?\nДядя - плохой."
                } 
            },
            { 
                {
                    "Today my grandmother took\nme to church.\nIt is a strange and boring place.",
                    "Сегодня бабушка взяла меня\nс собой в церковь.\nЭто странное и скучное\nместо."
                }, 
                {
                    "My grandmother took me to\nchurch. They said that all\npeople will die, that I will die,\nthat we are all bad and that\nI am bad too. I had to pray\nand ask for forgiveness.\nI DON'T WANT TO GO THERE!",
                    "Бабушка повела меня в\nцерковь. Там говорили, что\nвсе люди умрут, что я умру,\nчто мы все плохие и что я\nтоже плохой. Нужно было\nмолиться и просить\nпрощения.\nЯ НЕ ХОЧУ ТУДА ХОДИТЬ!"
                } 
            },
            { 
                {
                    "Uncle Yokki came to visit us\ntoday. Everyone had fun when\nhe was drunk and dancing on\nthe table. But grandma didn't\nlike it.\nGrandma doesn't like drunks.",
                    "Сегодня к нам в гости\nприходил дядя Йоуко.\nВсем было весело, когда он\nпьяный танцевал на столе.\nНо бабушке это не\nпонравилось.\nБабушка не любит пьяных."
                },
                {
                    "Uncle Jouko came, Grandma\nsaid that he would end badly\nand the bottle would kill him.\nGrandma doesn't like anyone.\nShe said that he would end\nhis life in agony and commit\nsuicide.\nGrandma is EVIL!",
                    "Приходил дядя Йоуко,\nбабушка сказала, что он\nплохо кончит и его погубит\nбутылка. Бабушке никто\nне нравится. Она сказала,\nчто он закончит свою жизнь\nв муках и станет\nсамоубийцей.\nБабушка ЗЛАЯ!"
                } 
            },
            { 
                {
                    "The doctor said that I have\nbad dreams about my\ngrandmother because I feel\nguilty. I should visit her more\noften. I don't really like to do\nit because she grumbles\nall the time.",
                    "Доктор сказал, что я вижу\nплохие сны про бабушку\nпотому, что испытываю\nчувство вины. Я должен чаще\nездить к ней в гости.\nЯ не очень люблю это делать\nпотому, что она\nвсе время ворчит."
                },
                {
                    "They think grandma is good,\nbut she's bad! She's a witch!\nShe's always walking on my\nceiling! I see her outside my\nwindow!\nI know she's EVIL!\nSHE'S A WITCH!",
                    "Они думают, что бабушка\nхорошая, но она плохая!\nЭто ведьма!\nОна постоянно ходит\nпо моему потолку!\nЯ вижу ее за окном!\nЯ знаю - она ЗЛАЯ!\nОНА ВЕДЬМА!"
                } 
            },
            { 
                {
                    "Doctor Hollberg came.\nHe said he could cure me,\nbut I had to be obedient and\ntake pills. The pills would\nhelp me come back\nfrom the bad world.",
                    "Приходил доктор Холлберг.\nОн сказал, что может меня\nвылечить, но я должен быть\nпослушным и пить\nтаблетки. Они помогут мне\nвозвращаться из\nнехорошего мира."
                }, 
                {
                    "I'm forbidden to do anything!\nI'm not allowed to hide my\nhands under the blanket!\nI'm not allowed to go near my\nparents' room when their bed\ncreaks! If I take a shower,\nsomeone is always standing\nbehind me!\nAND I HAVE TO EAT\nTHESE DAMN PILLS!!!",
                    "Мне всё запрещают!\nМне нельзя прятать руки\nпод одеялом! Мне нельзя\nподходить к комнате\nродителей, когда у них\nскрипит кровать!\nЕсли я принимаю душ, то\nкто-нибудь обязательно\nстоит за спиной!\nА ЕЩЕ Я ДОЛЖЕН ЖРАТЬ\r\nЭТИ ЧЕРТОВЫ ТАБЛЕТКИ!!!"
                } 
            },
            { 
                {
                    "I am admitted to hospital.\nI jump at every phone call or\nknock on the door.\nMy mother says it will be\nbetter this way. Maybe.",
                    "Меня кладут в больницу.\nЯ вздрагиваю от каждого\nтелефонного звонка или\nстука в дверь. Мама говорит,\nчто так будет лучше.\nМожет быть."
                }, 
                {
                    "They decided to lock me up in\na mental hospital! Damn, I'm\nsitting here shaking.\nEvery phone call, every knock\non the door - it could be the\ndoctors. They say it's better\nthis way! Ha!\nThey'd be better off dead!",
                    "Они решили упрятать меня\nв психушку! Черт, я сижу и\nтрясусь.\nКаждый телефонный звонок,\nкаждый стук в дверь - это\nмогут быть врачи.\nОни говорят, что так будет\nлучше! Ха!\nЛучше бы было им умереть!"
                } 
            },
            { 
                {
                    "Today I finally came home\nfrom the hospital!\nIt was so nice to eat\nhome-cooked food and go for\na walk! The doctor said I was\nalmost healthy, although\nflare-ups could return\r\nin the future.",
                    "Сегодня я наконец-то\nвернулся домой из больницы!\nКак приятно поесть\nдомашней еды и погулять!\nДоктор сказал, что я\nпочти здоров, хотя\nобострения могут вернуться\nв будущем."
                }, 
                {
                    "Fuck! They finally let me go!\nThey forbid everything in this\nfucking hospital!\nThey'll feed me some pills\nand be happy.\nI know I'm okay.",
                    "Бля! Наконец-то меня\nотпустили!\nВ этой сраной больнице все\nзапрещают! Накормят\nтаблетками и рады.\nЯ-то знаю,\nчто со мной все в порядке."
                } 
            },
            { 
                {
                    "Today my parents went on\nvacation. They deserved it.\nMom didn't want to leave me,\nbut the illness has receded,\nmore than two years have\npassed, I don't think anything\nbad will happen.",
                    "Сегодня родители уехали в\nотпуск. Они это заслужили.\nМама не хотела меня\nоставлять, но болезнь\nотступила, прошло больше\nдвух лет, не думаю,\nчто случится что-то\nдурное."
                }, 
                {
                    "Finally, I'm alone!\nWithout this care, without this\nsupervision. Damn, I haven't\nbeen a child for a long time!\nI've been healthy for a long\ntime! I'm going to have a\nblast this summer!",
                    "Наконец-то я остался один!\nБез этой опеки, без этого\nнадзора. Черт, я давно уже\nне ребенок! Я давно уже здоров!\nОторвусь в это лето\nпо полной!"
                } 
            },
            { 
                {
                    "I remember how my father\nand I were messing around in\nthe garage, fixing the car and\nlistening to music. I think\nI can still sometimes hear\nhis footsteps.",
                    "Помню, как мы с отцом\nвозились в гараже, чинили\nмашину и слушали музыку.\nМне кажется, что я до сих\nпор иногда слышу,\nкак звучат его шаги."
                }, 
                {
                    "I accidentally broke the bottle\nthat dad kept in the garage.\nIf he finds out, he'll kill me.\nI hear his footsteps...\nHe's searching.",
                    "Я случайно разбил бутылку,\nкоторую папа хранил\nв гараже. Если он узнает,\nто убьет меня.\nЯ слышу его шаги...\nОн ищет меня."
                } 
            },
            { 
                {
                    "I think it's starting again.\nToday I woke up to the sound\nof dripping water, but there\nwas no dripping anywhere.\nAnd footsteps in the garage,\nalthough there is no one there.\nIs my childhood illness\ncoming back?",
                    "Мне кажется, что это снова\nначинается. Сегодня я\nпроснулся от звука\nкапающей воды, но нигде\nничего не капало. А еще\nшаги в гараже, хотя там\nникого нет. Неужели мое\nдетское заболевание\nвозвращается?"
                },
                {
                    "It seems I see it all again.\nIt's like an epiphany!\nAs if back then, in the\nhospital, they stitched up\nmy third eye. Never mind,\nI'll get back at them\nfor everything.",
                    "Похоже я снова вижу все это.\nЭто похоже на прозрение!\nБудто тогда, в больнице,\nони зашили мне третий\nглаз.\nНичего, я им за все отомщу."
                } 
            },
            { 
                {
                    "Found Dr. Hollberg's phone\nnumber. Unfortunately,\nmy pills are no longer\nofficially available.\nBut he promised to help.\nIt's a big risk for him,\nso he'll hide the medicine\nin different places and\nsend me the coordinates\nof these places by mail.",
                    "Нашел телефон доктора\nХоллберга. К сожалению\nмоих таблеток больше нет\nв официальной продаже.\nНо, он обещал помочь.\nЭто большой риск для него,\nпоэтому он будет прятать\nлекарство в разных местах\nи отсылать мне по почте\nкоординаты этих мест."
                },
                {
                    "Yes, I have regained my power!\nNow I know it for sure!\nBut for now I still need pills\nto return. The fool of a doctor\r\nagreed to sell them to me\r\nillegally. True, I will have to\r\nrun around, finding where\r\nhe hid the next dose.",
                    "Да, я снова обрел свою силу!\nТеперь знаю это наверняка!\nНо пока мне все еще нужны\nтаблетки, чтобы\r\nвозвращаться.\r\nДурак-доктор согласился\r\nнелегально продавать мне их.\r\nПравда придется побегать,\r\nотыскивая куда он\r\nприпрятал очередную дозу."
                } 
            },
            { 
                {
                    "The doctor said that in\naddition to taking pills,\nI also need psychotherapy.\nAs if there is a feeling of guilt\nhanging over me and I need to\nget rid of it. To do this,\nI need to help people, do good\ndeeds. Maybe this will help\nme overcome the disease.",
                    "Доктор сказал, что помимо\nприема таблеток, мне еще\nнужна психотерапия.\nБудто бы надо мной висит\nчувство вины и я должен\nизбавиться от него.\nДля этого нужно помогать\nлюдям, совершать хорошие\nпоступки.\nВозможно это поможет мне\nпобедить болезнь."
                },
                {
                    "It's all the fault of these\nbastards who want to make me\ntheir servant! What, Mom?\nNot drink alcohol?\nWhat, Grandma?\nTake you to church?\nWhat, Mr. Teimo?\nDeliver your leaflets?\nGO TO HELL!\nIT'S ALL BECAUSE OF YOU!",
                    "Всему виной эти ублюдки,\nкоторые хотят сделать из\nменя своего слугу!\nЧто, мама?\nНе пить алкоголь?\nЧто, бабушка?\nОтвезти тебя в церковь?\nЧто мистер Теймо?\nРазвезти твои листовки?\nКАТИТЕСЬ К ЧЕРТУ!\nВСЕ ИЗ-ЗА ВАС!"
                } 
            },
        };
    }
}
