import * as React from 'react';
import { Route } from 'react-router-dom';
import { Layout } from './components/Layout';
import Home from './components/Home';
import AddPhoto from './components/AddPhoto';
import Gallery from './components/Gallery';

export const routes = <Layout>
    <Route exact path='/' component={ Home } />
    <Route path='/addphoto' component={ AddPhoto } />
    <Route path='/gallery' component={ Gallery } />
</Layout>;