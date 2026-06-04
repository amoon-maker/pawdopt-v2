// Apply saved theme before paint to avoid flash
(function () {
    const saved = localStorage.getItem('pawdopt-theme');
    if (saved === 'dark') document.documentElement.setAttribute('data-theme', 'dark');
})();

// ── Translations ──────────────────────────────────────────────────────────────
const i18n = {
    en: {
        'nav-home': 'Home',
        'nav-adopt': 'Adopt',
        'nav-rehome': 'Rehome',
        'nav-care': 'Care Guide',
        'nav-about': 'About Us',
        'nav-cta': 'Login / Register',
        'hero-badge': "Quebec's pet adoption platform",
        'hero-title': 'Give a new life to<br>an animal with<br><span class="hero-title-brand">Pawdopt</span>',
        'hero-subtitle': 'Pet adoption and rehoming are both vital aspects of animal welfare. Find your perfect companion or safely rehome your pet through our trusted platform.',
        'hero-btn-adopt': 'Adopt Now',
        'hero-btn-rehome': 'Rehome a Pet',
        'hero-stats-label': 'pets adopted',
        'how-eyebrow': 'SIMPLE PROCESS',
        'how-title': 'Adopt or Rehome in 3 easy steps',
        'how-step1-title': 'Create your profile',
        'how-step1-desc': 'Register and set up your adopter or rehomer account in minutes',
        'how-step2-title': 'Browse & match',
        'how-step2-desc': 'Filter pets by breed, size, age and location to find the right fit',
        'how-step3-title': 'Start the process',
        'how-step3-desc': "Submit a structured application and connect with the pet's owner",
        'featured-eyebrow': 'AVAILABLE NOW',
        'featured-title': 'Take a look at some of our pets',
        'featured-see-all': 'See all pets →',
        'pet-badge-new': 'New',
        'tag-male': 'Male',
        'tag-female': 'Female',
        'tag-large': 'Large',
        'tag-medium': 'Medium',
        'tag-small': 'Small',
        'btn-more-info': 'More Info',
        'cta-title': 'Find your perfect companion',
        'cta-desc': 'Our smart matching system helps you find a pet that fits your lifestyle, home size, and personality — because every adoption should be a forever story.',
        'cta-btn': 'Start matching →',
        'care-eyebrow': 'LEARN & PREPARE',
        'care-title': 'Pet care guides',
        'care-dog-title': 'Dog care essentials',
        'care-dog-desc': 'Nutrition, training, health tips for new dog owners',
        'care-cat-title': 'Cat care essentials',
        'care-cat-desc': 'Behavior, diet, and environment for happy cats',
        'care-small-title': 'Small pet guides',
        'care-small-desc': 'Rabbits, hamsters, birds and more',
        'test-eyebrow': 'COMMUNITY',
        'test-title': 'What people say about us',
        'faq-eyebrow': 'GOT QUESTIONS?',
        'faq-title': 'Frequently asked questions',
        'faq-q1': 'Why are you only helping dogs and cats?',
        'faq-a1': 'We started with dogs and cats as they represent the largest share of animals needing rehoming in Quebec. We plan to expand to other animals in future releases.',
        'faq-q2': 'Will you carry out a home check before adoption?',
        'faq-a2': 'Our platform facilitates direct communication between adopters and rehomers. A home visit is encouraged and can be arranged between parties, though it is not mandatory for all listings.',
        'faq-q3': 'How do I know my rehomed pet is safe?',
        'faq-a3': "All adopter profiles are verified and we provide a structured application process. You can review the adopter's profile, ask questions, and arrange a meet-and-greet before making a decision.",
        'faq-q4': 'Can I message the pet owner before applying?',
        'faq-a4': 'Yes! Registered users can send messages directly to pet owners or shelters through our in-platform messaging system before submitting a formal adoption application.',
        'footer-tagline': "Quebec's trusted pet adoption and rehoming platform.",
        'footer-explore': 'Explore',
        'footer-company': 'Company',
        'footer-contact': 'Contact',
        'footer-privacy': 'Privacy',
        'footer-terms': 'Terms & Conditions',
        'footer-copyright': '© 2026 Pawdopt. All rights reserved.',
        'terms-title': 'Terms & Conditions',
        'terms-close': 'I understand',
        'rehome-breadcrumb-list': 'List a pet',
        'rehome-step1': 'Pet info',
        'rehome-step2': 'Images',
        'rehome-step3': 'Character',
        'rehome-step4': 'Key facts',
        'rehome-step5': 'Location',
        'rehome-step6': 'Story',
        'rehome-step7': 'Documents',
        'rehome-step8': 'Confirm',
        'rehome-title': 'Tell us about your pet',
        'rehome-step-pill': 'Step 1 of 8',
        'rehome-step-desc': 'Basic information. Your progress is saved after each step.',
        'rehome-label-name': 'Pet Name',
        'rehome-label-species': 'Species',
        'rehome-label-breed': 'Breed',
        'rehome-label-age': 'Age',
        'rehome-label-gender': 'Gender',
        'rehome-label-size': 'Size',
        'rehome-label-color': 'Color',
        'rehome-label-weight': 'Weight (kg)',
        'rehome-label-microchip': 'Microchipped?',
        'rehome-label-reason': 'Reason for Rehoming',
        'rehome-select-default': 'Select...',
        'rehome-opt-dog': 'Dog',
        'rehome-opt-cat': 'Cat',
        'rehome-opt-other': 'Other',
        'rehome-opt-yes': 'Yes',
        'rehome-opt-no': 'No',
        'rehome-opt-unknown': 'Unknown',
        'rehome-placeholder-reason': 'Moving to a smaller apartment that does not allow pets...',
        'rehome-btn-continue': 'Save & Continue →',
        'rehome-btn-back': 'Back',
        'rehome-breadcrumb-images': 'Add photos',
        'rehome-img-title': 'Add photos of your pet',
        'rehome-img-step-pill': 'Step 2 of 8',
        'rehome-img-step-desc': 'Great photos increase adoption chances significantly.',
        'rehome-img-drop-title': 'Drag your photos here',
        'rehome-img-drop-sub': 'or click to browse files',
        'rehome-img-drop-hint': 'JPG · PNG · WebP  ·  Max 5 MB each  ·  Up to 6 photos',
        'rehome-img-counter-label': 'photos added',
        'rehome-img-tips-title': 'Photo tips',
        'rehome-img-tip1': 'Use natural lighting for clearer, warmer photos',
        'rehome-img-tip2': 'Include a clear face shot as the first (main) photo',
        'rehome-img-tip3': 'Show different angles and environments',
        'rehome-img-tip4': '3–6 photos gets the best response from adopters',
    },
    fr: {
        'nav-home': 'Accueil',
        'nav-adopt': 'Adopter',
        'nav-rehome': 'Confier',
        'nav-care': 'Guide de soins',
        'nav-about': 'À propos',
        'nav-cta': "Connexion / S'inscrire",
        'hero-badge': "La plateforme d'adoption d'animaux du Québec",
        'hero-title': 'Donnez une nouvelle vie à<br>un animal avec<br><span class="hero-title-brand">Pawdopt</span>',
        'hero-subtitle': "L'adoption et le réhébergement d'animaux sont des aspects essentiels du bien-être animal. Trouvez votre compagnon idéal ou confiez votre animal en toute sécurité grâce à notre plateforme.",
        'hero-btn-adopt': 'Adopter maintenant',
        'hero-btn-rehome': 'Confier un animal',
        'hero-stats-label': 'animaux adoptés',
        'how-eyebrow': 'PROCESSUS SIMPLE',
        'how-title': 'Adoptez ou confiez en 3 étapes simples',
        'how-step1-title': 'Créez votre profil',
        'how-step1-desc': 'Inscrivez-vous et configurez votre compte en quelques minutes',
        'how-step2-title': 'Parcourez & trouvez',
        'how-step2-desc': 'Filtrez par race, taille, âge et localisation pour trouver le bon compagnon',
        'how-step3-title': 'Commencez le processus',
        'how-step3-desc': "Soumettez une demande structurée et connectez-vous avec le propriétaire de l'animal",
        'featured-eyebrow': 'DISPONIBLES MAINTENANT',
        'featured-title': 'Découvrez quelques-uns de nos animaux',
        'featured-see-all': 'Voir tous les animaux →',
        'pet-badge-new': 'Nouveau',
        'tag-male': 'Mâle',
        'tag-female': 'Femelle',
        'tag-large': 'Grand',
        'tag-medium': 'Moyen',
        'tag-small': 'Petit',
        'btn-more-info': "Plus d'infos",
        'cta-title': 'Trouvez votre compagnon idéal',
        'cta-desc': "Notre système de correspondance intelligent vous aide à trouver un animal qui correspond à votre style de vie, la taille de votre logement et votre personnalité — car chaque adoption devrait être une histoire pour la vie.",
        'cta-btn': 'Commencer →',
        'care-eyebrow': 'APPRENEZ ET PRÉPAREZ-VOUS',
        'care-title': 'Guides de soins pour animaux',
        'care-dog-title': 'Soins essentiels pour chiens',
        'care-dog-desc': 'Nutrition, dressage, conseils santé pour les nouveaux propriétaires',
        'care-cat-title': 'Soins essentiels pour chats',
        'care-cat-desc': 'Comportement, alimentation et environnement pour des chats heureux',
        'care-small-title': 'Guides pour petits animaux',
        'care-small-desc': 'Lapins, hamsters, oiseaux et plus encore',
        'test-eyebrow': 'COMMUNAUTÉ',
        'test-title': 'Ce que disent nos utilisateurs',
        'faq-eyebrow': 'DES QUESTIONS?',
        'faq-title': 'Questions fréquemment posées',
        'faq-q1': 'Pourquoi aidez-vous seulement les chiens et les chats?',
        'faq-a1': "Nous avons commencé avec les chiens et les chats car ils représentent la plus grande part des animaux nécessitant un réhébergement au Québec. Nous prévoyons d'étendre à d'autres animaux dans les prochaines versions.",
        'faq-q2': "Effectuez-vous une visite à domicile avant l'adoption?",
        'faq-a2': "Notre plateforme facilite la communication directe entre adoptants et cédants. Une visite à domicile est encouragée et peut être organisée entre les parties, bien qu'elle ne soit pas obligatoire pour toutes les annonces.",
        'faq-q3': 'Comment savoir que mon animal réhébergé est en sécurité?',
        'faq-a3': "Tous les profils d'adoptants sont vérifiés et nous proposons un processus de candidature structuré. Vous pouvez consulter le profil de l'adoptant, poser des questions et organiser une rencontre avant de prendre une décision.",
        'faq-q4': "Puis-je contacter le propriétaire de l'animal avant de postuler?",
        'faq-a4': "Oui ! Les utilisateurs inscrits peuvent envoyer des messages directement aux propriétaires d'animaux ou aux refuges via notre système de messagerie intégré, avant de soumettre une demande d'adoption formelle.",
        'footer-tagline': "La plateforme québécoise de confiance pour l'adoption et le réhébergement d'animaux.",
        'footer-explore': 'Explorer',
        'footer-company': 'Compagnie',
        'footer-contact': 'Contact',
        'footer-privacy': 'Confidentialité',
        'footer-terms': 'Termes et conditions',
        'footer-copyright': '© 2026 Pawdopt. Tous droits réservés.',
        'terms-title': 'Termes et Conditions',
        'terms-close': 'Je comprends',
        'rehome-breadcrumb-list': 'Publier un animal',
        'rehome-step1': 'Infos animal',
        'rehome-step2': 'Images',
        'rehome-step3': 'Caractère',
        'rehome-step4': 'Infos clés',
        'rehome-step5': 'Localisation',
        'rehome-step6': 'Histoire',
        'rehome-step7': 'Documents',
        'rehome-step8': 'Confirmer',
        'rehome-title': 'Parlez-nous de votre animal',
        'rehome-step-pill': 'Étape 1 sur 8',
        'rehome-step-desc': 'Informations de base. Votre progression est sauvegardée après chaque étape.',
        'rehome-label-name': "Nom de l'animal",
        'rehome-label-species': 'Espèce',
        'rehome-label-breed': 'Race',
        'rehome-label-age': 'Âge',
        'rehome-label-gender': 'Sexe',
        'rehome-label-size': 'Taille',
        'rehome-label-color': 'Couleur',
        'rehome-label-weight': 'Poids (kg)',
        'rehome-label-microchip': 'Micropucé?',
        'rehome-label-reason': 'Raison du réhébergement',
        'rehome-select-default': 'Sélectionner...',
        'rehome-opt-dog': 'Chien',
        'rehome-opt-cat': 'Chat',
        'rehome-opt-other': 'Autre',
        'rehome-opt-yes': 'Oui',
        'rehome-opt-no': 'Non',
        'rehome-opt-unknown': 'Inconnu',
        'rehome-placeholder-reason': "Déménagement dans un appartement plus petit qui n'accepte pas les animaux...",
        'rehome-btn-continue': 'Sauvegarder & Continuer →',
        'rehome-btn-back': 'Retour',
        'rehome-breadcrumb-images': 'Ajouter des photos',
        'rehome-img-title': 'Ajoutez des photos de votre animal',
        'rehome-img-step-pill': 'Étape 2 sur 8',
        'rehome-img-step-desc': "De bonnes photos augmentent considérablement les chances d'adoption.",
        'rehome-img-drop-title': 'Glissez vos photos ici',
        'rehome-img-drop-sub': 'ou cliquez pour parcourir',
        'rehome-img-drop-hint': 'JPG · PNG · WebP  ·  Max 5 Mo  ·  Jusqu\'à 6 photos',
        'rehome-img-counter-label': 'photos ajoutées',
        'rehome-img-tips-title': 'Conseils photos',
        'rehome-img-tip1': 'Utilisez la lumière naturelle pour des photos plus nettes',
        'rehome-img-tip2': 'Incluez une photo du visage claire comme première photo',
        'rehome-img-tip3': 'Montrez différents angles et environnements',
        'rehome-img-tip4': '3 à 6 photos obtiennent la meilleure réponse des adoptants',
    }
};

function applyLang(lang) {
    document.querySelectorAll('[data-i18n]').forEach(function (el) {
        const key = el.getAttribute('data-i18n');
        if (i18n[lang][key] !== undefined) el.textContent = i18n[lang][key];
    });
    document.querySelectorAll('[data-i18n-html]').forEach(function (el) {
        const key = el.getAttribute('data-i18n-html');
        if (i18n[lang][key] !== undefined) el.innerHTML = i18n[lang][key];
    });
    document.querySelectorAll('#termsModal [data-lang]').forEach(function (el) {
        el.style.display = el.getAttribute('data-lang') === lang ? '' : 'none';
    });
    document.documentElement.lang = lang === 'fr' ? 'fr' : 'en';
    localStorage.setItem('pawdopt-lang', lang);
}

document.addEventListener('DOMContentLoaded', function () {

    // ── Dark mode toggle ──────────────────────────────
    const darkBtn = document.querySelector('.nav-icon-btn[title="Toggle dark mode"]');
    if (darkBtn) {
        const updateIcon = (isDark) => {
            darkBtn.innerHTML = isDark
                ? `<svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                     <circle cx="12" cy="12" r="5"/><line x1="12" y1="1" x2="12" y2="3"/>
                     <line x1="12" y1="21" x2="12" y2="23"/><line x1="4.22" y1="4.22" x2="5.64" y2="5.64"/>
                     <line x1="18.36" y1="18.36" x2="19.78" y2="19.78"/><line x1="1" y1="12" x2="3" y2="12"/>
                     <line x1="21" y1="12" x2="23" y2="12"/><line x1="4.22" y1="19.78" x2="5.64" y2="18.36"/>
                     <line x1="18.36" y1="5.64" x2="19.78" y2="4.22"/>
                   </svg>`
                : `<svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                     <path d="M21 12.79A9 9 0 1 1 11.21 3 7 7 0 0 0 21 12.79z"/>
                   </svg>`;
        };

        const isDark = () => document.documentElement.getAttribute('data-theme') === 'dark';
        updateIcon(isDark());

        darkBtn.addEventListener('click', function () {
            const next = isDark() ? 'light' : 'dark';
            document.documentElement.setAttribute('data-theme', next);
            localStorage.setItem('pawdopt-theme', next);
            updateIcon(next === 'dark');
        });
    }

    // ── Language toggle ───────────────────────────────
    const langBtn = document.getElementById('langToggle');
    if (langBtn) {
        let currentLang = localStorage.getItem('pawdopt-lang') || 'en';
        applyLang(currentLang);
        langBtn.textContent = currentLang === 'en' ? 'FR' : 'EN';

        langBtn.addEventListener('click', function () {
            currentLang = currentLang === 'en' ? 'fr' : 'en';
            applyLang(currentLang);
            langBtn.textContent = currentLang === 'en' ? 'FR' : 'EN';
        });
    }

    // ── FAQ accordion ─────────────────────────────────
    document.querySelectorAll('.faq-btn').forEach(function (btn) {
        btn.addEventListener('click', function () {
            const id = btn.getAttribute('data-faq');
            const answer = document.getElementById('faq-' + id);
            const isOpen = btn.getAttribute('aria-expanded') === 'true';

            document.querySelectorAll('.faq-btn').forEach(b => b.setAttribute('aria-expanded', 'false'));
            document.querySelectorAll('.faq-answer').forEach(a => a.classList.remove('open'));

            if (!isOpen) {
                btn.setAttribute('aria-expanded', 'true');
                answer.classList.add('open');
            }
        });
    });

});
