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
            "My notes... they're gone. It's all him! He stole them! I have to find him!",
            "Мои записи... они пропали. Это все он! Он их украл! Я должен его найти!"
        };

        public static readonly string[,] FINAL_PAGE = new string[,]
        {
            { // false ending
                "My parents won't come back. They left 10 years ago. I'm tired of waiting for them. I have to end all this.",
                "Родители не вернутся. Они уехали 10 лет назад. Я устал их ждать. Я должен все это закончить."
            },
            { // true ending
                "My parents sent me a postcard! They are fine and will be home soon!",
                "Родители прислали открытку! С ними все хорошо и скоро они будут дома!"
            }
        };

        public static readonly string[,,] PAGES = new string[,,]
        {
            {
                {
                    "Today I was taken to the hospital. They say that I have a headache. It's strange because my head doesn't hurt at all. And my mother was upset and cried. I don't want her to cry, but sometimes I hear her crying.",
                    "Сегодня меня водили в больницу. Говорят, что у меня болит голова. Это странно потому, что она у меня совсем не болит. А мама расстроилась и плакала. Я не хочу, что бы она плакала, но иногда слышу, как она плачет."
                },
                {
                    "I hear strange things. I see strange things. Sometimes I go to a strange world. I don't usually like it, but today it was fun. The barn was burning so brightly!",
                    "Я слышу странное. Я вижу странное. Иногда я перемещаюсь в странный мир. Обычно мне это не нравится, но сегодня было весело. Коровник горел так ярко!"
                }
            },
            {
                {
                    "Uncle Toivo came to visit today. My parents say that he drinks a lot and that's why his wife left him and now there's no one to raise Penti. They said that Penti is becoming brainless. I don't understand what that means.",
                    "Сегодня в гости приходил дядя Тойво. Родители говорят, что он много пьет и поэтому от него ушла жена, и теперь некому воспитывать Пенти. Они сказали, что Пенти становится безголовым. Не понимаю, что это значит."
                }, 
                {
                    "Uncle Toivo now lives alone. What did he do to his wife? Why did Penty lose his head? Uncle is bad.",
                    "Дядя Тойво теперь живет один. Что он сделал со своей женой? Почему Пенти остался без головы? Дядя - плохой."
                } 
            },
            { 
                {
                    "Today my grandmother took me to church. It is a strange and boring place.",
                    "Сегодня бабушка взяла меня с собой в церковь. Это странное и скучное место."
                }, 
                {
                    "My grandmother took me to church. They said that all people will die, that I will die, that we are all bad and that I am bad too. I had to pray and ask for forgiveness. I DON'T WANT TO GO THERE!",
                    "Бабушка повела меня в церковь. Там говорили, что все люди умрут, что я умру, что мы все плохие и что я тоже плохой. Нужно было молиться и просить прощения. Я НЕ ХОЧУ ТУДА ХОДИТЬ!"
                } 
            },
            { 
                {
                    "Uncle Yokki came to visit us today. Everyone had fun when he was drunk and dancing on the table. But grandma didn't like it. Grandma doesn't like drunks.",
                    "Сегодня к нам в гости приходил дядя Йоуко. Всем было весело, когда он пьяный танцевал на столе. Но бабушке это не понравилось. Бабушка не любит пьяных."
                },
                {
                    "Uncle Jouko came, Grandma said that he would end badly and the bottle would kill him. Grandma doesn't like anyone. But Grandma didn't like that. She said that he would end his life in agony and commit suicide. Grandma is EVIL!",
                    "Приходил дядя Йоуко, бабушка сказала, что он плохо кончит и его погубит бутылка. Бабушке никто не нравится. Но бабушке это не понравилось. Она сказала, что он закончит свою жизнь в муках и станет самоубийцей. Бабушка ЗЛАЯ!"
                } 
            },
            { 
                {
                    "The doctor said that I have bad dreams about my grandmother because I feel guilty. I should visit her more often. I don't really like to do it because she grumbles all the time.",
                    "Доктор сказал, что я вижу плохие сны про бабушку потому, что испытываю чувство вины. Я должен чаще ездить к ней в гости. Я не очень люблю это делать потому, что она все время ворчит."
                },
                {
                    "They think grandma is good, but she's bad! She's a witch! She's always walking on my ceiling! I see her outside my window! I know she's EVIL! SHE'S A WITCH!",
                    "Они думают, что бабушка хорошая, но она плохая! Это ведьма! Она постоянно ходит по моему потолку! Я вижу ее за окном! Я знаю - она ЗЛАЯ! ОНА ВЕДЬМА!"
                } 
            },
            { 
                {
                    "Doctor Hollberg came. He said he could cure me, but I had to be obedient and take pills. The pills would help me come back from the bad world.",
                    "Приходил доктор Холлберг. Он сказал, что может меня вылечить, но я должен быть послушным и пить таблетки. Таблетки помогут мне возвращаться из нехорошего мира."
                }, 
                {
                    "I'm forbidden to do anything! I'm not allowed to hide my hands under the blanket! I'm not allowed to go near my parents' room when their bed creaks! If I take a shower, someone is always standing behind me! AND I HAVE TO EAT THESE DAMN PILLS!!!",
                    "Мне всё запрещают! Мне нельзя прятать руки под одеялом! Мне нельзя подходить к комнате родителей, когда у них скрипит кровать! Если я принимаю душ, то кто-нибудь обязательно стоит за спиной! А ЕЩЕ Я ДОЛЖЕН ЖРАТЬ ЭТИ ЧЕРТОВЫ ТАБЛЕТКИ!!!"
                } 
            },
            { 
                {
                    "I am admitted to hospital. I jump at every phone call or knock on the door. My mother says it will be better this way. Maybe.",
                    "Меня кладут в больницу. Я вздрагиваю от каждого телефонного звонка или стука в дверь. Мама говорит, что так будет лучше. Может быть."
                }, 
                {
                    "They decided to lock me up in a mental hospital! Damn, I'm sitting here shaking. Every phone call, every knock on the door - it could be the doctors. They say it's better this way! Ha! They'd be better off dead!",
                    "Они решили упрятать меня в психушку! Черт, я сижу и трясусь. Каждый телефонный звонок, каждый стук в дверь - это могут быть врачи. Они говорят, что так будет лучше! Ха! Лучше бы было им умереть!"
                } 
            },
            { 
                {
                    "Today I finally came home from the hospital! It was so nice to eat home-cooked food and go for a walk! The doctor said I was almost healthy, although flare-ups could return in the future.",
                    "Сегодня я наконец-то вернулся домой из больницы! Как приятно поесть домашней еды и погулять! Доктор сказал, что я почти здоров, хотя обострения могут вернуться в будущем."
                }, 
                {
                    "Fuck! They finally let me go! They forbid everything in this fucking hospital! They'll feed me some pills and be happy.\nI know I'm okay.",
                    "Бля! Наконец-то меня отпустили! В этой сраной больнице все запрещают! Накормят таблетками и рады.\nЯ-то знаю, что со мной все в порядке."
                } 
            },
            { 
                {
                    "Today my parents went on vacation. They deserved it. Mom didn't want to leave me, but the illness has receded, more than two years have passed, I don't think anything bad will happen.",
                    "Сегодня родители уехали в отпуск. Они это заслужили. Мама не хотела меня оставлять, но болезнь отступила, прошло больше двух лет, не думаю, что случится что-то дурное."
                }, 
                {
                    "Finally, I'm alone! Without this care, without this supervision. Damn, I haven't been a child for a long time! I've been healthy for a long time! I'm going to have a blast this summer!",
                    "Наконец-то я остался один! Без этой опеки, без этого надзора. Черт, я давно уже не ребенок! Я давно уже здоров! Оторвусь в это лето по полной!"
                } 
            },
            { 
                {
                    "I remember how my father and I were messing around in the garage, fixing the car and listening to music. I think I can still sometimes hear his footsteps. ",
                    "Помню, как мы с отцом возились в гараже, чинили машину и слушали музыку. Мне кажется, что я до сих пор иногда слышу, как звучат его шаги."
                }, 
                {
                    "I accidentally broke the bottle that dad kept in the garage. If he finds out, he'll kill me. I hear his footsteps... He's searching.",
                    "Я случайно разбил бутылку, которую папа хранил в гараже. Если он узнает, то убьет меня. Я слышу его шаги... Он ищет."
                } 
            },
            { 
                {
                    "I think it's starting again. Today I woke up to the sound of dripping water, but there was no dripping anywhere. And footsteps in the garage, although there is no one there. Is my childhood illness coming back?",
                    "Мне кажется, что это снова начинается. Сегодня я проснулся от звука капающей воды, но нигде ничего не капало. А еще шаги в гараже, хотя там никого нет. Неужели мое детское заболевание возвращается?"
                },
                {
                    "It seems I see it all again. It's like an epiphany! As if back then, in the hospital, they stitched up my third eye. Never mind, I'll get back at them for everything.",
                    "Похоже я снова вижу все это. Это похоже на прозрение! Будто тогда, в больнице, они зашили мне третий глаз. Ничего, я им за все отомщу."
                } 
            },
            { 
                {
                    "Found Dr. Hollberg's phone number. Unfortunately, my pills are no longer officially available. But he promised to help. It's a big risk for him, so he'll hide the medicine in different places and send me the coordinates of these places by mail.",
                    "Нашел телефон доктора Холлберга. К сожалению моих таблеток больше нет в официальной продаже. Но, он обещал помочь. Это большой риск для него, поэтому он будет прятать лекарство в разных местах и отсылать мне по почте координаты этих мест."
                },
                {
                    "Yes, I have regained my power! Now I know it for sure! But for now I still need pills to return. The fool of a doctor agreed to sell them to me illegally. True, I will have to run around, finding where he hid the next dose.",
                    "Да, я снова обрел свою силу! Теперь знаю это наверняка! Но пока мне все еще нужны таблетки, чтобы возвращаться. Дурак-доктор согласился нелегально продавать мне их. Правда придется побегать, отыскивая куда он припрятал очередную дозу."
                } 
            },
            { 
                {
                    "The doctor said that in addition to taking pills, I also need psychotherapy. As if there is a feeling of guilt hanging over me and I need to get rid of it. To do this, I need to help people, do good deeds. Maybe this will help me overcome the disease.",
                    "Доктор сказал, что помимо приема таблеток, мне еще нужна психотерапия. Будто бы надо мной висит чувство вины и я должен избавиться от него. Для этого нужно помогать людям, совершать хорошие поступки. Возможно это поможет мне победить болезнь."
                },
                {
                    "It's all the fault of these bastards who want to make me their servant! What, Mom? Not drink alcohol? What, Grandma? Take you to church? What, Mr. Teimo? Deliver your leaflets? GO TO HELL! IT'S ALL BECAUSE OF YOU!",
                    "Всему виной эти ублюдки, которые хотят сделать из меня своего слугу! Что, мама? Не пить алкоголь? Что, бабушка? Отвезти тебя в церковь? Что мистер Теймо? Развезти твои листовки? КАТИТЕСЬ К ЧЕРТУ! ВСЕ ИЗ-ЗА ВАС!"
                } 
            },
        };
    }
}
