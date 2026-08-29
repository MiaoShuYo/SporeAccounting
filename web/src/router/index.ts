import {createMemoryHistory, createRouter} from 'vue-router'

import Login from '../pages/Login.vue'
import Main from "../pages/Main.vue";
import PrimaryCurrency from "../pages/config/PrimaryCurrency.vue";
import notfound from "../pages/404.vue";
import Index from "../pages/Index.vue";
import IncomeExpenditureType from "../pages/config/IncomeExpenditureType.vue";
import Budget from "../pages/config/budget.vue";
import AccountBook from "../pages/AccountBook.vue";
import Record from '../pages/Record.vue'
import Report from '../pages/Report.vue'

const routes = [
    {
        name: 'main', path: '/', component: Main, children: [
            {
                name: 'index', path: '', component: Index
            },
            {
                name: 'accountBook', path: 'accountBook', component: AccountBook
            },
            {
                name: 'record',
                path: 'record/:accountBookId',
                component: Record
            },
            {
                name: 'report',
                path: 'report',
                component: Report
            },
            {
                name: 'config', path: 'config', children: [
                    {
                        name: 'primaryCurrency',
                        path: 'primaryCurrency',
                        component: PrimaryCurrency
                    },
                    {
                        name: 'incomeExpenditureType',
                        path: 'incomeExpenditureType',
                        component: IncomeExpenditureType
                    },
                    {
                        name: 'budget',
                        path: 'budget',
                        component: Budget
                    }
                ]
            },
        ]
    },
    {name: 'login', path: '/login', component: Login},
    {name: 'notFound', path: '/:pathMatch(.*)*', component: notfound},
]

// 判断路由是否在子路由中
const isRouteInChildren = (routes: any[], routeName: string): boolean => {
    for (const route of routes) {
        if (route.name === routeName) {
            return true;
        }
        if (route.children && isRouteInChildren(route.children, routeName)) {
            return true;
        }
    }
    return false;
};

export const router = createRouter({
    history: createMemoryHistory(),
    routes,
})

export const navigateTo = (routeName: string) => {
    router.push({name: routeName});
};

router.beforeEach((to, _from, next) => {
    // 如果是404页面，直接放行
    if (to.name === 'notFound') {
        next();
        return;
    }
    
    // 如果是登录页面，直接放行
    if (to.name === 'login') {
        next();
        return;
    }
    
    // 如果路由名称为空或不在定义的路由中，跳转到404
    if (!to.name || !isRouteInChildren(routes, to.name as string)) {
        next({name: 'notFound'});
        return;
    }
    
    // 检查token，如果没有token则跳转到登录页
    const token = localStorage.getItem('token')
    if (token) {
        next();
    } else {
        next({name: 'login'});
    }
})