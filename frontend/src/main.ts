import { createApp } from 'vue'
import App from './App.vue'
import router from './router'
import 'bootstrap'
import './app.scss'

import { library } from '@fortawesome/fontawesome-svg-core'
import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome'
import { faCheck, faXmark } from '@fortawesome/free-solid-svg-icons'
library.add(faCheck, faXmark)

createApp(App).use(router).component('font-awesome-icon', FontAwesomeIcon).mount('#app')
