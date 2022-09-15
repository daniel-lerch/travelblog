import { createRouter, createWebHistory, RouteRecordRaw } from 'vue-router'
import HomeView from './views/HomeView.vue'
import AdminView from './views/AdminView.vue'

const routes: Array<RouteRecordRaw> = [
  {
    path: '/',
    name: 'Home',
    component: HomeView
  },
  {
    path: '/register',
    name: 'Register',
    component: () => import(/* webpackChunkName: "register" */ './views/RegisterView.vue')
  },
  {
    path: '/registered',
    name: 'Registered',
    component: () => import(/* webpackChunkName: "register" */ './views/RegisteredView.vue')
  },
  {
    path: '/admin',
    name: 'Admin',
    component: AdminView
  }
]

const router = createRouter({
  history: createWebHistory(window.resourceBasePath),
  routes
})

export default router
