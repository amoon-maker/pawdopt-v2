// Shared Pawdopt pet data — referenced by Adopt, PetDetail, and AdopterProfile pages
window.PAWDOPT_PETS = [
    {
        id:1, name:'Pitter', species:'dog', breed:'Pit Bull', size:'large',
        ageNum:3, ageUnit:'yr', gender:'male', province:'QC', city:'Montreal',
        image:'Pitter.png', emoji:'🐕', badge:'new', vaccinated:true, neutered:true, microchipped:false,
        colors:'linear-gradient(135deg,#fde8d8 0%,#e8d5f0 100%)',
        desc:'Friendly and playful. Great for a home with a yard. Excellent with older children.',
        story:'Pitter is a 3-year-old Pit Bull mix who loves nothing more than outdoor adventures and cuddle sessions. He was found as a stray and has been lovingly fostered for over a year. He is house-trained, knows basic commands, and gets along beautifully with older children. He would thrive in a home with a yard where he can run and play freely.',
        traits:['Friendly','Playful','House-trained','Good with older kids','Loves the outdoors'],
        rehomer:{ name:'Sophie L.', city:'Montreal, QC', since:'March 2026', avatar:'S' }
    },
    {
        id:2, name:'Hero', species:'cat', breed:'DLH', size:'small',
        ageNum:2, ageUnit:'yr', gender:'male', province:'AB', city:'Calgary',
        image:'Hero.png', emoji:'🐈', badge:'', vaccinated:true, neutered:false, microchipped:true,
        colors:'linear-gradient(135deg,#d8eef5 0%,#e8f0e8 100%)',
        desc:'Quiet and curious indoor cat. Perfect for apartment living. Very low maintenance.',
        story:'Hero is a calm and observant 2-year-old Domestic Longhair who loves sitting by windows and watching the world go by. He is very low-maintenance, litter-trained, and perfectly suited for apartment life. He will warm up quickly once he trusts you and loves to be petted on his own terms.',
        traits:['Independent','Quiet','Litter-trained','Apartment-friendly','Gentle'],
        rehomer:{ name:'Marco T.', city:'Calgary, AB', since:'January 2026', avatar:'M' }
    },
    {
        id:3, name:'Magie', species:'dog', breed:'Shiba Inu', size:'medium',
        ageNum:14, ageUnit:'mo', gender:'female', province:'ON', city:'Toronto',
        image:'Magie.png', emoji:'🐕‍🦺', badge:'', vaccinated:true, neutered:true, microchipped:true,
        colors:'linear-gradient(135deg,#d8e8f5 0%,#ddf0e8 100%)',
        desc:'Lives with 2 children and gets along beautifully. Social and energetic pup.',
        story:'Magie is a lively 14-month-old Shiba Inu who has grown up with two children aged 7 and 13. She is energetic, social, and loves interactive play. She is house-trained, leash-trained, and responds well to positive reinforcement. Magie would do best in an active family who can give her the attention and exercise she loves.',
        traits:['Social','Energetic','Leash-trained','Good with kids','Loves to play'],
        rehomer:{ name:'Priya K.', city:'Toronto, ON', since:'February 2026', avatar:'P' }
    },
    {
        id:4, name:'Felix', species:'cat', breed:'Domestic SH', size:'small',
        ageNum:5, ageUnit:'yr', gender:'female', province:'BC', city:'Vancouver',
        image:'Felix.png', emoji:'🐱', badge:'', vaccinated:false, neutered:true, microchipped:false,
        colors:'linear-gradient(135deg,#fff0d8 0%,#f5e8f0 100%)',
        desc:'Loves to cuddle and follow you around. Very social and affectionate cat.',
        story:'Felix (yes, she kept her name!) is a sweet 5-year-old Domestic Shorthair who loves to be wherever you are. She follows her humans from room to room and is happiest when near people. She is perfect for someone who works from home or wants a constant companion. She has never been aggressive and gets along with most cats.',
        traits:['Affectionate','Social','Lap cat','Good with other cats','People-oriented'],
        rehomer:{ name:'Jen H.', city:'Vancouver, BC', since:'April 2026', avatar:'J' }
    },
    {
        id:5, name:'Buddy', species:'dog', breed:'Golden Retriever', size:'large',
        ageNum:4, ageUnit:'yr', gender:'male', province:'QC', city:'Quebec City',
        image:'Buddy.png', emoji:'🐶', badge:'', vaccinated:true, neutered:true, microchipped:true,
        colors:'linear-gradient(135deg,#fdf8d8 0%,#f8ead8 100%)',
        desc:'Gentle giant who loves fetch and long walks. Great with families and other dogs.',
        story:"Buddy is everything you'd expect from a Golden — warm, patient, and endlessly loving. At 4 years old, he is past the hyper puppy phase and has settled into a wonderfully balanced dog. He loves long walks, fetch, and swimming. He is great with other dogs and a true family companion. His owner is relocating internationally and sadly cannot bring him along.",
        traits:['Gentle','Patient','Loves fetch','Good with other dogs','Family dog'],
        rehomer:{ name:'David R.', city:'Quebec City, QC', since:'May 2026', avatar:'D' }
    },
    {
        id:6, name:'Luna', species:'cat', breed:'Maine Coon', size:'medium',
        ageNum:1, ageUnit:'yr', gender:'female', province:'QC', city:'Laval',
        image:'Luna.png', emoji:'🐈‍⬛', badge:'new', vaccinated:true, neutered:false, microchipped:false,
        colors:'linear-gradient(135deg,#ead8f8 0%,#d8e8f8 100%)',
        desc:'Elegant and quiet. Loves high places and cozy naps. Fully litter-trained.',
        story:'Luna is a beautiful 1-year-old Maine Coon who carries herself with the grace of a much older cat. She loves perching on high shelves, watching birds through the window, and curling up for long afternoon naps. She is litter-trained and very clean. Luna would do well in a calm home, ideally with someone who appreciates a quiet and elegant companion.',
        traits:['Elegant','Independent','Quiet','Loves high perches','Clean'],
        rehomer:{ name:'Camille B.', city:'Laval, QC', since:'June 2026', avatar:'C' }
    },
    {
        id:7, name:'Rex', species:'dog', breed:'German Shepherd', size:'large',
        ageNum:7, ageUnit:'yr', gender:'male', province:'ON', city:'Ottawa',
        image:'Rex.png', emoji:'🦮', badge:'urgent', vaccinated:true, neutered:true, microchipped:true,
        colors:'linear-gradient(135deg,#e0ecd5 0%,#f0e8d5 100%)',
        desc:'Loyal and protective senior dog. Needs experienced owner. Currently in foster care.',
        story:'Rex is a distinguished 7-year-old German Shepherd with a calm and loyal temperament. He has had professional training and responds to commands reliably. He is protective of his family but gentle once trust is established. Rex does best with an experienced dog owner who understands the breed. He is currently in loving foster care and needs his forever home urgently.',
        traits:['Loyal','Protective','Professionally trained','Calm','Experienced-owner preferred'],
        rehomer:{ name:'Foster Care', city:'Ottawa, ON', since:'January 2026', avatar:'F' }
    },
    {
        id:8, name:'Mochi', species:'dog', breed:'Pomeranian', size:'small',
        ageNum:8, ageUnit:'mo', gender:'female', province:'BC', city:'Victoria',
        image:'Mochi.png', emoji:'🐕', badge:'new', vaccinated:true, neutered:false, microchipped:false,
        colors:'linear-gradient(135deg,#fce8f0 0%,#f8e8fc 100%)',
        desc:'Tiny ball of fluff and energy! Loves cuddles and squeaky toys. Very playful.',
        story:'Mochi is an 8-month-old Pomeranian with a huge personality packed into a tiny body. She is playful, curious, and absolutely loves to cuddle after a big play session. She is still learning commands and responds well to treats. She would be a perfect fit for a first-time owner or a family looking for a small, lively companion.',
        traits:['Playful','Curious','Cuddly','First-timer-friendly','Small breed'],
        rehomer:{ name:'Emma P.', city:'Victoria, BC', since:'May 2026', avatar:'E' }
    },
    {
        id:9, name:'Shadow', species:'cat', breed:'Russian Blue', size:'small',
        ageNum:3, ageUnit:'yr', gender:'male', province:'AB', city:'Edmonton',
        image:'Shadow.png', emoji:'🐈', badge:'', vaccinated:true, neutered:true, microchipped:true,
        colors:'linear-gradient(135deg,#d8e4fc 0%,#d8f0f4 100%)',
        desc:'Calm and independent. Adjusts well to new environments. Good with other cats.',
        story:'Shadow is a serene 3-year-old Russian Blue with a silvery coat and an incredibly calm demeanour. He adapts quickly to new homes and gets along well with other cats. He is not a lap cat but loves to be in the same room as his humans. Shadow would be a great fit for someone who wants a low-drama, elegant feline companion.',
        traits:['Calm','Independent','Good with other cats','Adaptable','Low-maintenance'],
        rehomer:{ name:'Nathan V.', city:'Edmonton, AB', since:'March 2026', avatar:'N' }
    },
    {
        id:10, name:'Bella', species:'dog', breed:'Labrador', size:'large',
        ageNum:2, ageUnit:'yr', gender:'female', province:'QC', city:'Sherbrooke',
        image:'Bella.png', emoji:'🐶', badge:'new', vaccinated:true, neutered:true, microchipped:true,
        colors:'linear-gradient(135deg,#eaf8d8 0%,#d8f0f8 100%)',
        desc:'Cheerful lab mix who loves swimming and playing ball. Great family dog.',
        story:'Bella is a 2-year-old Labrador full of enthusiasm and joy. She loves swimming, hiking, and chasing a ball for hours. She is fully house-trained, leash-trained, and gets along beautifully with children and other dogs. Her current owner is expecting a baby and sadly cannot provide the active lifestyle Bella deserves. She would be an ideal match for a sporty, active family.',
        traits:['Energetic','Loves water','Good with children','Leash-trained','Social'],
        rehomer:{ name:'Lucie F.', city:'Sherbrooke, QC', since:'April 2026', avatar:'L' }
    },
    {
        id:11, name:'Oliver', species:'cat', breed:'British Shorthair', size:'medium',
        ageNum:6, ageUnit:'yr', gender:'male', province:'MB', city:'Winnipeg',
        image:'Oliver.png', emoji:'🐱', badge:'urgent', vaccinated:false, neutered:true, microchipped:false,
        colors:'linear-gradient(135deg,#e8dde8 0%,#dde8e0 100%)',
        desc:'Mellow senior cat looking for a quiet home. Loves lap time and window watching.',
        story:"Oliver is a 6-year-old British Shorthair who has the personality of a wise and gentle old soul. He loves long naps, window watching, and settling into laps for extended petting sessions. He does not like loud noise or chaotic environments. Oliver is the ideal companion for a quiet household, a senior owner, or someone working from home who wants a peaceful furry friend.",
        traits:['Gentle','Lap cat','Quiet home needed','Senior cat','Window watcher'],
        rehomer:{ name:'Rita O.', city:'Winnipeg, MB', since:'February 2026', avatar:'R' }
    },
    {
        id:12, name:'Nala', species:'dog', breed:'Husky', size:'large',
        ageNum:3, ageUnit:'yr', gender:'female', province:'SK', city:'Saskatoon',
        image:'Nala.png', emoji:'🐕', badge:'new', vaccinated:true, neutered:false, microchipped:true,
        colors:'linear-gradient(135deg,#d8e8fc 0%,#fce8d8 100%)',
        desc:'Stunning Husky with bright blue eyes. Needs an active family and lots of exercise.',
        story:"Nala is a breathtaking 3-year-old Siberian Husky with piercing blue eyes and a playful spirit. She is highly intelligent, loves running and exploring, and has a strong pack mentality. Nala needs an active family who can provide at least 2 hours of exercise daily. She does well with other dogs and children but requires an experienced owner who understands the Husky breed's energy and independence.",
        traits:['Active','Intelligent','Good with other dogs','Needs exercise','Experienced-owner preferred'],
        rehomer:{ name:'Karl S.', city:'Saskatoon, SK', since:'May 2026', avatar:'K' }
    }
];
